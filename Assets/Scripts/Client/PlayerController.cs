using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.Stats;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Client.World.Entity;
using SoulboundBackend.Core;
using System;
using Unity.VisualScripting;
using Logger = SoulboundBackend.Common.Logging.Logger;
using UnityEngine;
using UnityEngine.InputSystem;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Client.World;
using SoulboundBackend.Common;
using SoulboundBackend.Client.World.BlockSystem;
using System.Collections.Generic;

namespace SoulboundBackend.Client {
	public class PlayerController : LivingEntity, IGameInitializable<PlayerController> {
		private static readonly Logger logger = Logger.CreateInstance();
		public override Type entityScriptType => typeof(PlayerController);
		public override string prefabDefinitionID => "johnny";
		[SerializeField] private InputHandler inputHandler;
		public InputHandler InputHandler => inputHandler;

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


		private ItemUsageHandler itemUsageHandler;
		public ItemUsageHandler ItemUsageHandler => itemUsageHandler!;
#nullable enable
		public ItemStack? MainHandStack { get; private set; }

		public bool CanAttack { get; set; } = true;

		public float facing => playerPhysics.facing;
		public Vector2 center => Physics.Collider.bounds.center;
		public BlockPos blockPos => GameManager.instance.Level.ToBlockPos(this.position);
		public ChunkBlockPos chunkBlockPos => blockPos.ToChunkBlockPos(GameManager.instance.Level.ChunkXAt(position));

		public Vector2 itemDropForce {
			get {
				Vector2 force = new Vector2(3f, 4f);
				force.x *= facing;
				return force;
			}
		}

		public float MaxBlockReach => 5f;

		public PlayerController OnGameInit() {
			inputHandler = GameObject.Instantiate(inputHandler).OnGameInit(this);
			inventory = GameManager.instance.UIManager.InstantiateInUILevel(inventory).GetComponent<InventoryController>().OnGameInit(this);
			playerPhysics = gameObject.GetComponent<PlayerPhysics>().OnGameInit(this);
			itemUsageHandler = new ItemUsageHandler(this);
			itemUsageHandler.Register<IConsumable>(ItemUseTrigger.RightClick, (consumable, stack) => consumable.Consume(stack));
			foreach (ItemUseTrigger trigger in Enum.GetValues(typeof(ItemUseTrigger))) {
				itemUsageHandler.Register<IAttackPerformer>(trigger, (attackPerformer, stack) => {
					InvocationHelper.If(CanAttack, () => attackPerformer.PerformAttack(trigger));
				});
			}
			itemUsageHandler.Register<IPlaceable>(ItemUseTrigger.LeftHold, (placeable, stack) => {
				Level level = GameManager.instance.Level;
				BlockPos blockPos = level.ToBlockPos(inputHandler.MouseWorldPosition);

				if (CanPlaceBlockAt(blockPos)) {
					level.SetBlock(blockPos, placeable.Place(stack, blockPos));
				}
			});
			return this;
		}

		protected override void Update() {
			base.Update();
			animator.SetFloat("horizontalSpeed", Mathf.Abs(rb.linearVelocityX));
		}

		public void SetMainHandItem(ItemStack? itemStack) {
			if (MainHandStack == itemStack) {
				return;
			}
			static void InvokeStatItem(ItemStack? itemStack, Action<IStatProvider> statProviderAction) {
				if (itemStack?.item is IStatProvider statProvider && statProvider.applyInstantStatsOnHoverOrSelect) {
					statProviderAction.Invoke(statProvider);
				}
			}
			//InvokeStatItem(MainHandStack, statProvider => statProvider.RevokeStats(this.stats));
			MainHandStack = itemStack;
			//InvokeStatItem(MainHandStack, statProvider => statProvider.ApplyStats(this.stats));
		}

		[InputAction("ItemUse", Priority = 5)]
		internal void OnLeftClick() => RequestSuppressedMainHandUse(ItemUseTrigger.LeftClick);

		[InputAction("ItemUse", Priority = 5)]
		internal void OnRightClick() {
			UnityEngine.Debug.Log("right click");
			RequestMainHandUse(ItemUseTrigger.RightClick, null);
		}

		// POTENTIAL FEATUREIMPL: add Reach int stat

		[InputAction("ItemUse", Priority = 5)]
		internal void OnLeftHold() {
			if (MainHandStack != null) {
				RequestSuppressedMainHandUse(ItemUseTrigger.LeftHold);
			} else {
				InputHandler.RequestAction(new InputActionRequest("BlockBreak", 5, () => {
					Level level = GameManager.instance.Level;
					Vector2 worldMousePos = inputHandler.MouseWorldPosition;
					BlockPos blockPos = level.ToBlockPos(worldMousePos);

					if (IsInBlockReach((Vector2)blockPos) && level.BlockAt(blockPos) != Blocks.air) {
						level.BreakBlock(blockPos, BreakSource.Player);
					}
				}, null));
			}
		}

		[InputAction("ItemUse", Priority = 5)]
		internal void OnRightHold() {
			RequestMainHandUse(ItemUseTrigger.RightHold, null);
		}

		private void RequestSuppressedMainHandUse(ItemUseTrigger trigger) {
			Item? usedItem = MainHandStack?.item;
			RequestMainHandUse(trigger, null);
			InputHandler.RequestAction(new("ItemUse", 5, () => itemUsageHandler.HandleInput(trigger, MainHandStack), null));
			if (usedItem is IPlaceable) {
				InputHandler.BlockContext("BlockBreak", () => !InputHandler.LeftHold);
			}
		}

		private void RequestMainHandUse(ItemUseTrigger trigger, Action? callback) {
			InputHandler.RequestAction(new("ItemUse", 5, () => itemUsageHandler.HandleInput(trigger, MainHandStack), callback));
		}

		public bool CanPlaceBlockAt(BlockPos blockPos) {
			Vector2 worldPos = (Vector2)blockPos;
			return IsInBlockReach(worldPos)
				   && GameManager.instance.Level.BlockAt(blockPos) == Blocks.air;
		}

		public bool IsInBlockReach(Vector2 worldPos) {
			float dist = Vector2.Distance(worldPos, this.center);
			return dist <= MaxBlockReach && !GameManager.instance.Level.GetTilesCovered(playerPhysics.Collider.bounds).Contains(BlockPos.FromWorld(worldPos));
		}

		// since this is a lazy player entity addition, not all methods need implementation (for now)
		// TODO: properly implement player entity methods

		public override void Spawn(EntitySpawnData spawnData) {
			base.Spawn(spawnData);
			logger.LogInfo(null, "Player spawned at {}", spawnData.Get<Vector2>("position"));
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
			} catch (Exception e) {
				this.stats = new();
				logger.LogError(null, e);
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