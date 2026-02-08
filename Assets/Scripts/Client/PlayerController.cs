using NUnit.Framework.Internal;
using SoulboundBackend.Client.Combat;
using SoulboundBackend.Client.Concurrency;
using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.Stats;
using SoulboundBackend.Client.UI;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Client.World.EntitySystem;
using SoulboundBackend.Client.World.EntitySystem.SpawnData;
using SoulboundBackend.Common;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Bootstrap;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using Zenject;
using static PlayerInputActions;
using static Unity.VisualScripting.Member;
using static UnityEditor.PlayerSettings;
using Level = SoulboundBackend.Client.World.Level;
using Logger = SoulboundBackend.Common.Logging.Logger;

namespace SoulboundBackend.Client {
	public class PlayerController : Entity, IAttackPerformer, IItemConsumer, IUpdatable, IEntitySpawnable<PlayerSpawnData> {
		private static readonly Logger logger = Logger.CreateInstance();
		public override Type scriptType => typeof(PlayerController);
		public override EntityDescriptor descriptor => EntityDescriptorRegistry.ByType<PlayerController>();
		[SerializeField] private InventoryController inventory;
		[Obsolete] public InventoryController Inventory => inventory;
		private PlayerInventory _inventory;
		public PlayerInventory GetInventory() => _inventory;

		[SerializeField] private PlayerStats stats;
		public PlayerStats Stats => stats;
		IStatModificationHost IStatContextProvider.statModificationHost => stats;

		[Header("Internal")]
		[SerializeField] private Rigidbody2D rb;
		[SerializeField] private Animator animator;
		private InputHandler inputHandler;
		private PlayerPhysics playerPhysics;
		private AttackHandler attackHandler = null!;
		private AttackSource attackSource = null!;

		public bool isSpawned { get; private set; }


		private ItemUsageHandler itemUsageHandler;
		public ItemUsageHandler ItemUsageHandler => itemUsageHandler!;

#nullable enable
		public ItemStack? MainHandStack { get; private set; }

		public bool CanAttack { get; set; } = true;

		public Vector2 center => playerPhysics.Collider.bounds.center;

		public Vector2 itemDropForce {
			get {
				Vector2 force = new Vector2(3f, 4f);
				force.x *= facing.direction.x;
				return force;
			}
		}

		public float MaxBlockReach => 5f;

		private Level level = null!;
		private Vector2 mouseScreenPos;
		private Canvas canvas = null!;
		private Vector2 mouseWorldPos {
			get {
				Vector3 screenPos = mouseScreenPos;
				RectTransform rootTransform = canvas.GetComponent<RectTransform>();
				if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rootTransform, screenPos, Camera.main, out var worldPoint)) {
					return worldPoint;
				}
				screenPos.z = -Camera.main.transform.position.z;
				return Camera.main.ScreenToWorldPoint(screenPos);
			}
		}

		public override Facing facing => new(playerPhysics.facing);

		private bool leftHold;
		private bool rightHold;
		private ConcurrentActionResolver actionResolver = null!;

		[Inject]
		public void Construct(DiContainer container) {
			inputHandler = container.Resolve<InputHandler>();
			playerPhysics = container.Resolve<PlayerPhysics>();
			//inventory = container.Resolve<InventoryController>();
			_inventory = new PlayerInventory();
			inputHandler.RegisterInputEvent(inputHandler.GetAction("Toggle Inventory"), pausable: true, binding => {
				binding.Performed(_ => _inventory.TogglePopup());
			});
			level = container.Resolve<Level>();
			canvas = container.Resolve<Canvas>();
			RegisterItemUsageCandidates(container.Resolve<ItemUsageHandler>());
			actionResolver = container.Resolve<ConcurrentActionResolver>();

			attackSource = new AttackSource(2, 10, new PlayerMainHandAttack(),
				context => {
					var eventDispatcher = GetComponent<AttackEventDispatcher>();
					context.eventDispatcher = eventDispatcher;
					return AttackAnimatorChannel.FromDelegates(
						GetComponent<Animator>,
						() => eventDispatcher
					);
				},
				animator => animator.SetTrigger("attack")
			);

			inputHandler.RegisterInputEvent(inputHandler.GetAction("LeftClick"), pausable: true, binding => {
				binding.Performed(_ => OnLeftClick());
				binding.Performed(_ => leftHold = true);
				binding.Canceled(_ => leftHold = false);
			});
			inputHandler.RegisterInputEvent(inputHandler.GetAction("RightClick"), pausable: true, binding => {
				binding.Performed(_ => OnRightClick());
				binding.Performed(_ => rightHold = true);
				binding.Performed(_ => rightHold = true);
			});
			inputHandler.RegisterInputEvent(inputHandler.GetAction("MousePosition"), pausable: false, binding => {
				binding.Performed(context => mouseScreenPos = context.ReadValue<Vector2>());
			});
		}

		private void RegisterItemUsageCandidates(ItemUsageHandler? itemUsageHandler) {
			if (itemUsageHandler == null) {
				return;
			}
			this.itemUsageHandler = itemUsageHandler;

			itemUsageHandler.RegisterCapability<IConsumable>(ItemUseTrigger.RightClick, (consumable, stack) => consumable.StartConsume(this, stack));
			foreach (ItemUseTrigger trigger in Enum.GetValues(typeof(ItemUseTrigger))) {
				itemUsageHandler.RegisterCapability<IAttackSourceProvider>(trigger, (sourceProvider, stack) => {
					if (sourceProvider.GetAttackSource(trigger, out var attackSource)) {
						UnityEngine.Debug.Log("using attack item: "+ trigger);
						TryAttack(attackSource);
					}
				});
			}
			itemUsageHandler.RegisterCapability<IPlaceable>(ItemUseTrigger.LeftHold, (placeable, stack) => {
				BlockPos blockPos = (BlockPos)mouseWorldPos;

				if (CanPlaceBlockAt(blockPos)) {
					level.PlaceBlock(blockPos, placeable.Place(stack, blockPos));
				}
			});
			itemUsageHandler.RegisterCapability<IBreakingTool>(ItemUseTrigger.LeftHold, (tool, stack) => {
				BlockPos blockPos = (BlockPos)mouseWorldPos;

				if (IsInBlockReach((Vector2)blockPos)) {
					tool.TryBreak(blockPos, level, new PlayerToolBreakSource(this, tool));
				}
			});
		}

		public void TryAttack(AttackSource source) {
			if (attackHandler?.isHandlingAttack ?? false) {
				return;
			}
			attackHandler = new AttackHandler(source);
			attackHandler.StartAttack(this, null);
		}

		void IUpdatable.FrameUpdate(float deltaTime) {
			InvocationHelper.If(leftHold, OnLeftHold);
			InvocationHelper.If(rightHold, OnRightHold);
		}

		public void SetMainHandItem(ItemStack? itemStack) {
			if (MainHandStack == itemStack) {
				return;
			}
			MainHandStack = itemStack;
		}

		private void OnLeftClick() {
			RequestMainHandItemUse(ItemUseTrigger.LeftClick);
		}

		private void OnRightClick() {
			RequestMainHandItemUse(ItemUseTrigger.RightClick);
		}

		// POTENTIAL FEATUREIMPL: add Reach int stat

		private void OnLeftHold() {
			RequestMainHandItemUse(ItemUseTrigger.LeftHold);

			actionResolver.Submit(Request.New()
				.UnderToken(PlayerActionTokens.BlockBreak)
				.Execute(() => {
					Level level = Soulbound.instance.GetActiveLevel()!;
					BlockPos blockPos = (BlockPos)mouseWorldPos;
					Block? targetBlock = level.BlockAt(blockPos);

					if (0 >= targetBlock?.breakRequirement?.minBreakPower) {
						level.BreakBlock(blockPos, new PlayerToolBreakSource(this, null));
					}
				})
				.OnCondition(() => CanBreakBlockAt((BlockPos)mouseWorldPos))
				.Suppress(PlayerActionTokens.Attack, () => !leftHold)
			);

			actionResolver.Submit(Request.New()
				.UnderToken(PlayerActionTokens.Attack)
				.Execute(() => TryAttack(attackSource))
				.Suppress(PlayerActionTokens.BlockBreak, () => !leftHold)
			);
		}

		private void OnRightHold() {
			RequestMainHandItemUse(ItemUseTrigger.RightHold);
		}

		private void RequestMainHandItemUse(ItemUseTrigger trigger) {
			actionResolver.Submit(Request.New()
				.UnderToken(PlayerActionTokens.ItemUse)
				.Execute(() => {
					itemUsageHandler.HandleInput(trigger, MainHandStack);
				})
				.OnCondition(() => MainHandStack != null)
				.Suppress(PlayerActionTokens.BlockBreak, () => !leftHold)
				.Suppress(PlayerActionTokens.Attack, () => !leftHold)
			);
		}

		public void OnItemDisplayDestroyed(ItemStack stack) {
			if (stack == MainHandStack) {
				this.SetMainHandItem(null);
			}
		}

		public bool CanPlaceBlockAt(BlockPos blockPos) {
			Vector2 worldPos = (Vector2)blockPos;
			return IsInBlockReach(worldPos)
				   && Soulbound.instance.GetActiveLevel()!
						.BlockAt(blockPos) == Blocks.air;
		}

		public bool CanBreakBlockAt(BlockPos blockPos) {
			Vector2 worldPos = (Vector2)blockPos;
			return IsInBlockReach(worldPos)
				   && Soulbound.instance.GetActiveLevel()!
						.BlockAt(blockPos) != Blocks.air;
		}

		public bool IsInBlockReach(Vector2 worldPos) {
			Level level = Soulbound.instance.GetActiveLevel()!;
			float dist = Vector2.Distance(worldPos, this.center);
			return dist <= MaxBlockReach 
				&& !level.GetTilesCovered(playerPhysics.Collider.bounds)
						 .Contains((BlockPos)worldPos);
		}

		void IEntitySpawnable<PlayerSpawnData>.ApplySpawnData(PlayerSpawnData spawnData) {
			this.transform.position = spawnData.position;
			this.stats = new();
			this.isSpawned = true;
		}

		public override void Deserialize(SerializedEntity serialized) {
			base.Deserialize(serialized);
			//this.stats = new();
			//var properties = SerializedEntityPropertyList.From(serialized.properties);
			//inventory.Deserialize(properties.GetOrThrow<SerializedInventory>("inventory"));
		}

		public override SerializedEntity Serialize() {
			var serialized = base.Serialize();

			var properties = SerializedEntityPropertyList.From(serialized.properties);
			//properties.Set("inventory", inventory.Serialize());

			serialized.properties = properties;
			return serialized;
		}


		// since this is a lazy player entity addition, not all methods need implementation (for now)
		// TODO: properly implement player entity methods

		//public override void Spawn(EntitySpawnData spawnData) {
		//	base.Spawn(spawnData);
		//	logger.LogInfo("Player spawned at {}", spawnData.Get<Vector2>("position"));
		//	this.isSpawned = true;
		//}

		//public override void OnChunkLoaded() {
		//}

		//public override void OnChunkUnloaded() {
		//}

		//public override Bounds GetBounds() => this.GetColliderBounds();

		//public override void OnDeath() {
		//	logger.LogInfo("Player died");
		//}

		//public override void OnDamageTaken(float damage) {
		//	logger.LogInfo("Player has taken {} damage", damage);
		//}

		//public override void ApplySerializedProperties(ComponentSerializer properties) {
		//	base.ApplySerializedProperties(properties);
		//	this.stats = new();
		//	inventory.Deserialize(properties.GetOrThrow<SerializedInventory>("inventory"));
		//	//stats.UpdateInjectedMappings();
		//	//stats.MaxHealth.OnModifiersChanged += maxHealth => {
		//	//	bool wasFullHealth = this.currentHealth == this.maxHealth;
		//	//	this.maxHealth = maxHealth.GetProcessedValue();
		//	//	if (wasFullHealth) {
		//	//		this.currentHealth = this.maxHealth;
		//	//	}
		//	//};

		//	this.maxHealth = stats.maxHealth.GetProcessedValue();
		//	this.currentHealth = maxHealth;         // might cause problems later with OnDeath handling
		//											// If the player were to leave while having the death screen active,
		//											// Upon rejoining it would reset their health to max, completely overriding the death state.
		//											// Death screen implementation should be prioritized when deserializing.
		//}

		//public override ComponentSerializer GetSerializedProperties() {
		//	return base.GetSerializedProperties()
		//		//.Add("stats", this.stats)
		//		.Add("inventory", inventory.Serialize());
		//}

	}

}
