using NUnit.Framework.Internal;
using SoulboundBackend.Client.Combat;
using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.Stats;
using SoulboundBackend.Client.UI;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Client.World.EntitySystem;
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
	public class PlayerController : LivingEntity, IAttackPerformer {
		private static readonly Logger logger = Logger.CreateInstance();
		public override Type entityScriptType => typeof(PlayerController);
		public override string prefabDefinitionID => "johnny";
		public InputHandler inputHandler { get; private set; }

		[SerializeField] private InventoryController inventory;
		public InventoryController Inventory => inventory;

		[SerializeField] private PlayerStats stats;
		public PlayerStats Stats => stats;

		private PlayerPhysics playerPhysics;
		public PlayerPhysics Physics => playerPhysics;

		[Header("Internal")]
		[SerializeField] private Rigidbody2D rb;
		public Rigidbody2D Rigidbody => rb;
		[SerializeField] private Animator animator;
		public Animator Animator => animator;
		private AttackHandler attackHandler = null!;
		private AttackSource attackSource = null!;

		public bool isSpawned { get; private set; }


		private ItemUsageHandler itemUsageHandler;
		public ItemUsageHandler ItemUsageHandler => itemUsageHandler!;

#nullable enable
		public ItemStack? MainHandStack { get; private set; }

		public bool CanAttack { get; set; } = true;

		public override float facing { get => playerPhysics.facing; set => playerPhysics.facing = value; }
		public Vector2 center => Physics.Collider.bounds.center;
		public BlockPos blockPos => Soulbound.instance.GetActiveLevel()!.ToBlockPos(this.position);
		public ChunkBlockPos chunkBlockPos => blockPos.ToChunkBlockPos(Soulbound.instance.GetActiveLevel()!.ChunkXAt(position));

		public Vector2 itemDropForce {
			get {
				Vector2 force = new Vector2(3f, 4f);
				force.x *= facing;
				return force;
			}
		}

		public float MaxBlockReach => 5f;

		private Level level;
		private Vector2 mouseScreenPos;
		private Canvas canvas;
		private Vector2 mouseWorldPos {
			get {
				Vector3 screenPos = mouseScreenPos;
				RectTransform rootTransform = canvas.GetComponent<UIManager>().GetRootTransform();
				if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rootTransform, screenPos, Camera.main, out var worldPoint)) {
					return worldPoint;
				}
				screenPos.z = -Camera.main.transform.position.z;
				return Camera.main.ScreenToWorldPoint(screenPos);
			}
		}
		private bool leftHold;
		private bool rightHold;

		[Inject]
		public void Construct(DiContainer container) {
			this.inputHandler = container.Resolve<InputHandler>();
			this.playerPhysics = container.Resolve<PlayerPhysics>();
			this.inventory = container.Resolve<InventoryController>();
			this.level = container.Resolve<Level>();
			this.canvas = container.Resolve<Canvas>();
			RegisterItemUsageCandidates(container.Resolve<ItemUsageHandler>());

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

			inputHandler.RegisterInputEvent(inputHandler.GetAction("Player/LeftClick"), pausable: true, (action) => {
				action.performed += _ => OnLeftClick();
				action.performed += _ => leftHold = true;
				action.canceled += _ => leftHold = false;
			});
			inputHandler.RegisterInputEvent(inputHandler.GetAction("Player/RightClick"), pausable: true, (action) => {
				action.performed += _ => OnRightClick();
				action.performed += _ => rightHold = true;
				action.canceled += _ => rightHold = false;
			});
			inputHandler.RegisterInputEvent(inputHandler.GetAction("Player/MousePosition"), pausable: false, (action) => {
				action.performed += actionContext => mouseScreenPos = actionContext.ReadValue<Vector2>();
			});
		}

		private void RegisterItemUsageCandidates(ItemUsageHandler? itemUsageHandler) {
			if (itemUsageHandler == null) {
				return;
			}
			this.itemUsageHandler = itemUsageHandler;

			itemUsageHandler.RegisterCapability<IConsumable>(ItemUseTrigger.RightClick, (consumable, stack) => consumable.Consume(stack));
			foreach (ItemUseTrigger trigger in Enum.GetValues(typeof(ItemUseTrigger))) {
				itemUsageHandler.RegisterCapability<IAttackSourceProvider>(trigger, (sourceProvider, stack) => {
					if (sourceProvider.GetAttackSource(trigger, out var attackSource)) {
						UnityEngine.Debug.Log("using attack item: "+ trigger);
						TryAttack(attackSource);
					}
				});
			}
			itemUsageHandler.RegisterCapability<IPlaceable>(ItemUseTrigger.LeftHold, (placeable, stack) => {
				BlockPos blockPos = level.ToBlockPos(mouseWorldPos);

				if (CanPlaceBlockAt(blockPos)) {
					level.PlaceBlock(blockPos, placeable.Place(stack, blockPos));
				}
			});
			itemUsageHandler.RegisterCapability<IBreakingTool>(ItemUseTrigger.LeftHold, (tool, stack) => {
				BlockPos blockPos = level.ToBlockPos(mouseWorldPos);

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
			logger.LogInfo(null, "executing attack");
			attackHandler.StartAttack(this, null);
		}

		public override void EntityUpdate(float deltaTime) {
			base.EntityUpdate(deltaTime);
			InvocationHelper.If(leftHold, OnLeftHold);
			InvocationHelper.If(rightHold, OnRightHold);
		}

		public void SetMainHandItem(ItemStack? itemStack) {
			if (MainHandStack == itemStack) {
				return;
			}
			MainHandStack = itemStack;
		}

		[InputAction("ItemUse", Priority = 5)]
		internal void OnLeftClick() {
			RequestSuppressedMainHandUse(ItemUseTrigger.LeftClick);
		}

		[InputAction("ItemUse", Priority = 5)]
		internal void OnRightClick() {
			RequestMainHandUse(ItemUseTrigger.RightClick, null);
		}

		// POTENTIAL FEATUREIMPL: add Reach int stat

		[InputAction("ItemUse", Priority = 5)]
		internal void OnLeftHold() {
			if (MainHandStack != null) {
				RequestSuppressedMainHandUse(ItemUseTrigger.LeftHold);
			} else {
				if (CanBreakBlockAt((BlockPos)mouseWorldPos)) {
					inputHandler.RequestAction(new("BlockBreak", 5, () => {
						Level level = Soulbound.instance.GetActiveLevel()!;
						BlockPos blockPos = level.ToBlockPos(mouseWorldPos);
						Block? targetBlock = level.BlockAt(blockPos);

						if (0 >= targetBlock.breakRequirement?.minBreakPower) {
							level.BreakBlock(blockPos, new PlayerToolBreakSource(this, null));
						}
					}, null));
					inputHandler.BlockContext("PlayerAttack", () => !leftHold);
				} else {
					inputHandler.RequestAction(new("PlayerAttack", 5, () => TryAttack(attackSource), null));
				}
			}
		}

		[InputAction("ItemUse", Priority = 5)]
		internal void OnRightHold() {
			RequestMainHandUse(ItemUseTrigger.RightHold, null);
		}

		private void RequestSuppressedMainHandUse(ItemUseTrigger trigger) {
			Item? usedItem = MainHandStack?.item;
			RequestMainHandUse(trigger, null);
			if (usedItem is IPlaceable) {
				inputHandler.BlockContext("BlockBreak", () => !leftHold);
			}
		}

		private void RequestMainHandUse(ItemUseTrigger trigger, Action? callback) {
			inputHandler.RequestAction(new("ItemUse", 5, () => itemUsageHandler.HandleInput(trigger, MainHandStack), callback));
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
						.Contains(BlockPos.FromWorld(worldPos, level));
		}

		// since this is a lazy player entity addition, not all methods need implementation (for now)
		// TODO: properly implement player entity methods

		public override void Spawn(EntitySpawnData spawnData) {
			base.Spawn(spawnData);
			logger.LogInfo(null, "Player spawned at {}", spawnData.Get<Vector2>("position"));
			this.isSpawned = true;
		}

		public override void OnChunkLoaded() {
		}

		public override void OnChunkUnloaded() {
		}

		public override Bounds GetBounds() => this.GetColliderBounds();

		public override void OnDeath() {
			logger.LogInfo(null, "Player died");
		}

		public override void OnDamageTaken(float damage) {
			logger.LogInfo(null, "Player has taken {} damage", damage);
		}

		public override void ApplySerializedProperties(SerializedEntityPropertyList properties) {
			base.ApplySerializedProperties(properties);
			List<IItemSlot>? pendingAttachUpdates = null;
			try {
				pendingAttachUpdates = inventory.Deserialize(properties.GetOrThrow<SerializedInventory>("inventory"));
				this.stats = properties.GetOrThrow<PlayerStats>("stats");
			} catch (Exception) {
				this.stats = new();
			} finally {
				stats.UpdateInjectedMappings();
				stats.MaxHealth.OnModifiersChanged += maxHealth => {
					bool wasFullHealth = this.currentHealth == this.maxHealth;
					this.maxHealth = maxHealth.GetProcessedValue();
					if (wasFullHealth) {
						this.currentHealth = this.maxHealth;
					}
				};

				this.maxHealth = stats.MaxHealth.GetProcessedValue();
				this.currentHealth = maxHealth;         // might cause problems later with OnDeath handling
														// If the player were to leave while having the death screen active,
														// Upon rejoining it would reset their health to max, completely overriding the death state.
														// Death screen implementation should be prioritized when deserializing.
				pendingAttachUpdates?.ForEach(s => s.NotifyDeserializedHook());
			}
		}

		public override SerializedEntityPropertyList GetSerializedProperties() {
			return base.GetSerializedProperties()
				.Add("stats", this.stats)
				.Add("inventory", inventory.Serialize());
		}
	}

}