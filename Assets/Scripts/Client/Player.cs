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

#nullable enable

namespace SoulboundBackend.Client {
	public class Player : Entity, IAttackPerformer, IItemConsumer, IInputContext, IFrameUpdatableEntity {
		private static readonly AssetKey playerKey = new("player");
		private static readonly EntityDescriptor DESCRIPTOR = new("player", null);
		private readonly Inventory inventory;
		private readonly Hotbar hotbar;
		private readonly Canvas canvas;
		private readonly PlayerStats stats = new();
		private readonly ItemUsageHandler itemUsageHandler;
		[Obsolete] private readonly AttackSource attackSource;
		public PlayerStats Stats => stats;
		IStatModificationHost IStatContextProvider.statModificationHost => stats;
		private PlayerTransform playerTransform = null!;
		[Obsolete] private AttackHandler attackHandler;

		public ItemStack? MainHandStack { get; private set; }

		const float MAX_BLOCK_REACH = 5f;

		private Vector2 screenPointerPos;

		private bool isHoldingLeftClick;
		private bool isHoldingRightClick;
		[Obsolete] private ConcurrentActionResolver actionResolver = null!;

		public Player(Canvas canvas, Vector2 initialPos)
			: base(DESCRIPTOR, initialPos) {
			inventory = new Inventory();
			hotbar = new Hotbar();
			this.canvas = canvas;
			Soulbound.instance.GetInputManager().PushContext(this);

			actionResolver = new ConcurrentActionResolver();
			itemUsageHandler = new ItemUsageHandler(this);
			RegisterItemUsageCandidates();

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
			attackHandler = new AttackHandler(attackSource);
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
				screenPointerPos = inputEvent.context.ReadValue<Vector2>();
			}
			if (inputEvent.token.Equals(InputTokens.Mouse.leftClick)) {
				if (inputEvent.phase == InputActionPhase.Performed) {
					OnLeftClick();
					isHoldingLeftClick = true;
					return true;
				} else if (inputEvent.phase == InputActionPhase.Canceled) {
					isHoldingLeftClick = false;
					return true;
				}
			}
			if (inputEvent.token.Equals(InputTokens.Mouse.rightClick)) {
				if (inputEvent.phase == InputActionPhase.Performed) {
					OnRightClick();
					isHoldingRightClick = true;
					return true;
				} else if (inputEvent.phase == InputActionPhase.Canceled) {
					isHoldingRightClick = false;
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

		[Obsolete]
		private void RegisterItemUsageCandidates() {
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
				BlockPos blockPos = (BlockPos)GetWorldPointerPos();

				if (CanPlaceBlockAt(blockPos)) {
					level.PlaceBlock(blockPos, placeable.Place(stack, blockPos));
				}
			});
			itemUsageHandler.RegisterCapability<IBreakingTool>(ItemUseTrigger.LeftHold, (tool, stack) => {
				BlockPos blockPos = (BlockPos)GetWorldPointerPos();

				if (IsInBlockReach((Vector2)blockPos)) {
					tool.TryBreak(blockPos, level, new PlayerToolBreakSource(this, tool));
				}
			});
		}

		[Obsolete]
		public void TryAttack(AttackSource source) {
			if (attackHandler?.isHandlingAttack ?? false) return;
			attackHandler = new AttackHandler(source);
			attackHandler.StartAttack(this, null);
		}

		void IFrameUpdatableEntity.FrameUpdate() {
			if (isHoldingLeftClick) OnLeftHold();
			if (isHoldingRightClick) OnRightHold();
		}

		private void OnLeftClick() {
			RequestMainHandItemUse(ItemUseTrigger.LeftClick);
		}

		private void OnRightClick() {
			RequestMainHandItemUse(ItemUseTrigger.RightClick);

			// provisory
			level.InteractBlock((BlockPos)GetWorldPointerPos());
		}

		// POTENTIAL FEATUREIMPL: add Reach int stat

		private void OnLeftHold() {
			RequestMainHandItemUse(ItemUseTrigger.LeftHold);

			actionResolver.Submit(Request.New()
				.UnderToken(PlayerActionTokens.BlockBreak)
				.Execute(() => {
					BlockPos blockPos = (BlockPos)GetWorldPointerPos();
					Block? targetBlock = level.BlockAt(blockPos);

					if (0 >= targetBlock?.breakRequirement?.minBreakPower) {
						level.BreakBlock(blockPos, new PlayerToolBreakSource(this, null));
					}
				})
				.OnCondition(() => CanBreakBlockAt((BlockPos)GetWorldPointerPos()))
				.Suppress(PlayerActionTokens.Attack, () => !isHoldingLeftClick)
			);

			actionResolver.Submit(Request.New()
				.UnderToken(PlayerActionTokens.Attack)
				.Execute(() => TryAttack(attackSource))
				.Suppress(PlayerActionTokens.BlockBreak, () => !isHoldingLeftClick)
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
				.Suppress(PlayerActionTokens.BlockBreak, () => !isHoldingLeftClick)
				.Suppress(PlayerActionTokens.Attack, () => !isHoldingLeftClick)
			);
		}

		public bool CanPlaceBlockAt(BlockPos blockPos) {
			Vector2 worldPos = (Vector2)blockPos;
			return IsInBlockReach(worldPos)
				   && level?.BlockAt(blockPos) == Blocks.air;
		}

		public bool CanBreakBlockAt(BlockPos blockPos) {
			Vector2 worldPos = (Vector2)blockPos;
			return IsInBlockReach(worldPos)
				   && level?.BlockAt(blockPos) != Blocks.air;
		}

		public bool IsInBlockReach(Vector2 worldPos) {
			float dist = Vector2.Distance(worldPos, GetCenter());
			return dist <= MAX_BLOCK_REACH 
				&& !level.GetTilesCovered(playerTransform.Collider.bounds)
						 .Contains((BlockPos)worldPos);
		}

		public Inventory GetInventory() => inventory;
		public Hotbar GetHotbar() => hotbar;

		public Vector2 GetCenter() => playerTransform.Collider.bounds.center;

		public bool IsHoldingLeftClick() => isHoldingLeftClick;
		public bool IsHoldingRightClick() => isHoldingRightClick;

		public Vector2 GetScreenPointerPos() => screenPointerPos;
		public Vector2 GetWorldPointerPos() {
			Vector3 screenPos = screenPointerPos;
			RectTransform rootTransform = canvas.GetComponent<RectTransform>();
			bool inWorldPoint = RectTransformUtility.ScreenPointToWorldPointInRectangle(
				rootTransform,
				screenPos,
				Camera.main,
				out var worldPoint
			);
			if (inWorldPoint) return worldPoint;

			screenPos.z = -Camera.main.transform.position.z;
			return Camera.main.ScreenToWorldPoint(screenPos);
		}
	}
}
