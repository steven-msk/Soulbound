using SoulboundBackend.Client.Combat;
using SoulboundBackend.Client.Concurrency;
using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.SettingSystem;
using SoulboundBackend.Client.Stats;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.EntitySystem;
using SoulboundBackend.Common;
using SoulboundBackend.Core;
using SoulboundBackend.Core.AssetManagement;
using SoulboundBackend.Core.Debug.Logging;
using SoulboundBackend.Core.Resource;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using Level = SoulboundBackend.Client.World.Level;
using Logger = SoulboundBackend.Core.Debug.Logging.Logger;

namespace SoulboundBackend.Client {
	public class Player : Entity, IAttackPerformer, IItemConsumer, IUpdatable, IInputContext {
		private static readonly AssetKey playerKey = new("player");
		private static readonly EntityDescriptor DESCRIPTOR = new("player", null);
		private readonly Inventory inventory;
		private readonly Hotbar hotbar;

		private PlayerStats stats = new();
		public PlayerStats Stats => stats;
		IStatModificationHost IStatContextProvider.statModificationHost => stats;

		private PlayerTransform playerTransform;
		private AttackHandler attackHandler;
		private AttackSource attackSource;
		private ItemUsageHandler itemUsageHandler;

#nullable enable
		public ItemStack? MainHandStack { get; private set; }

		public Vector2 center => playerTransform.Collider.bounds.center;

		const float MAX_BLOCK_REACH = 5f;

		public Vector2 mouseScreenPos;
		private Canvas canvas = null!;
		public Vector2 mouseWorldPos {
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

		public bool leftHold;
		public bool rightHold;
		private ConcurrentActionResolver actionResolver = null!;

		public Player(Canvas canvas, Vector2 initialPos)
			: base(DESCRIPTOR, initialPos) {
			inventory = new Inventory();
			hotbar = new Hotbar();
			this.canvas = canvas;
			Soulbound.instance.GetInputManager().PushContext(this);

			inventory.GetSlot(0).SetStack(new ItemStack(Items.leavesBlock, 5));

			actionResolver = new ConcurrentActionResolver();
			RegisterItemUsageCandidates(new ItemUsageHandler(this));

			attackSource = new AttackSource(2, 10, new PlayerMainHandAttack(),
				context => {
					var eventDispatcher = playerTransform.GetComponent<AttackEventDispatcher>();
					context.eventDispatcher = eventDispatcher;
					return AttackAnimatorChannel.FromDelegates(
						playerTransform.GetComponent<Animator>,
						() => eventDispatcher
					);
				},
				animator => animator.SetTrigger("attack")
			);
		}

		public override void SetPos(Vector2 pos) => transform.SetPos(pos);
		public override Vector2 GetPos() => transform.GetPos();

		protected override IEntityTransform CreateTransform() {
			GameObject obj = GameObject.Instantiate(AssetManager.Resolve<GameObject>(playerKey));
			playerTransform = obj.GetComponent<PlayerTransform>();
			return playerTransform;
		}

		[PROTOTYPICAL]
		bool IInputContext.HandleInput(in InputEvent inputEvent) {
			if (inputEvent.token.Equals(InputTokens.Player.toggleInventory)) {
				inventory.Toggle();
				return true;
			}
			if (inputEvent.token.Equals(InputTokens.Player.changeHotbarSlot)
					&& inputEvent.phase == InputActionPhase.Performed) {
				int slotIndex = int.Parse(inputEvent.context.control.name) - 1;
				hotbar.SetMainSlot(slotIndex);
				return true;
			}
			if (inputEvent.token.Equals(InputTokens.Player.scrollHotbarSlot)
					&& inputEvent.phase == InputActionPhase.Performed) {
				float scrollDelta = inputEvent.context.ReadValue<float>();
				int nextSlot = hotbar.GetMainSlot() - (int)scrollDelta;

				if (nextSlot < 0) nextSlot += hotbar.GetSlotCount();
				nextSlot %= hotbar.GetSlotCount();
				hotbar.SetMainSlot(nextSlot);
				return true;
			}
			if (inputEvent.token.Equals(InputTokens.Mouse.position)) {
				mouseScreenPos = inputEvent.context.ReadValue<Vector2>();
			}
			if (inputEvent.token.Equals(InputTokens.Mouse.leftClick)) {
				if (inputEvent.phase == InputActionPhase.Performed) {
					OnLeftClick();
					leftHold = true;
					return true;
				} else if (inputEvent.phase == InputActionPhase.Canceled) {
					leftHold = false;
					return true;
				}
			}
			if (inputEvent.token.Equals(InputTokens.Mouse.rightClick)) {
				if (inputEvent.phase == InputActionPhase.Performed) {
					OnRightClick();
					rightHold = true;
					return true;
				} else if (inputEvent.phase == InputActionPhase.Canceled) {
					rightHold = false;
					return true;
				}
			}

			if (inputEvent.token.Equals(InputTokens.Player.move)) {
				if (inputEvent.phase == InputActionPhase.Performed) {
					playerTransform.SetVelocityX(inputEvent.context.ReadValue<Vector2>().x);
					return true;
				} else if (inputEvent.phase == InputActionPhase.Canceled) {
					playerTransform.SetVelocityX(0f);
					return true;
				}
			}
			if (inputEvent.token.Equals(InputTokens.Player.jump)
					&& inputEvent.phase == InputActionPhase.Performed) {
				playerTransform.Jump();
				return true;
			}
			return false;
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
			if (leftHold) OnLeftHold();
			if (rightHold) OnRightHold();
		}

		public void SetMainHandItem(ItemStack? itemStack) {
			if (MainHandStack == itemStack) return;
			MainHandStack = itemStack;
		}

		private void OnLeftClick() {
			RequestMainHandItemUse(ItemUseTrigger.LeftClick);
		}

		private void OnRightClick() {
			RequestMainHandItemUse(ItemUseTrigger.RightClick);

			// provisory
			level.InteractBlock((BlockPos)mouseWorldPos);
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
			return dist <= MAX_BLOCK_REACH 
				&& !level.GetTilesCovered(playerTransform.Collider.bounds)
						 .Contains((BlockPos)worldPos);
		}

		public Inventory GetInventory() => inventory;
		public Hotbar GetHotbar() => hotbar;

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
