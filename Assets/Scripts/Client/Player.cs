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
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

#nullable enable

namespace SoulboundBackend.Client {
	public class Player : Entity, IAttackPerformer, IInputContext, IFrameUpdatableEntity {
		private static readonly AssetKey playerKey = new("player");
		private static readonly EntityDescriptor DESCRIPTOR = new("player", null);
		private readonly Inventory inventory;
		private readonly Hotbar hotbar;
		private readonly PlayerStats stats = new();
		[Obsolete] private readonly AttackSource attackSource;
		public PlayerStats Stats => stats;
		private PlayerTransform playerTransform = null!;
		[Obsolete] private AttackHandler attackHandler;
		private new Vector2 initialPos;

		const float MAX_BLOCK_REACH = 5f;

		private Vector2 screenPointerPos;
		private bool isHoldingLeftClick;
		private bool isHoldingRightClick;

		public Player(Vector2 initialPos)
			: base(DESCRIPTOR, initialPos) {
			inventory = new Inventory();
			hotbar = new Hotbar();
			this.initialPos = initialPos;

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
				hotbar.SetMainSlotIndex(slotIndex);
				return true;
			}
			if (inputEvent.token.Equals(InputTokens.Player.scrollHotbarSlot)
					&& inputEvent.phase == InputActionPhase.Performed) {
				float scrollDelta = inputEvent.context.ReadValue<float>();
				int nextSlot = hotbar.GetMainSlotIndex() - (int)scrollDelta;

				if (nextSlot < 0) nextSlot += hotbar.GetSlotCount();
				nextSlot %= hotbar.GetSlotCount();
				hotbar.SetMainSlotIndex(nextSlot);
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
					OnLeftRelease();
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
					OnRightRelease();
					isHoldingRightClick = false;
					return true;
				}
			}

			if (inputEvent.token.Equals(InputTokens.Player.move)) {
				if (inputEvent.phase == InputActionPhase.Performed) {
					playerTransform.SetNormalVelocityX(inputEvent.context.ReadValue<Vector2>().x);
					return true;
				} else if (inputEvent.phase == InputActionPhase.Canceled) {
					playerTransform.SetNormalVelocityX(0f);
					return true;
				}
			}
			if (inputEvent.token.Equals(InputTokens.Player.jump)) {
				if (inputEvent.phase == InputActionPhase.Performed) {
					playerTransform.SetJumping(true);
					return true;
				} else if (inputEvent.phase == InputActionPhase.Canceled) {
					playerTransform.SetJumping(false);
					return true;
				}
			}

			if (inputEvent.Performed(InputTokens.Keyboard.Q)) {
				ThrowMainHandStack();
				return true;
			}
			return false;
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
			ItemStack? mainHandStack = GetMainHandStack();

			if (!TryUseItem(mainHandStack, ItemActionTrigger.LeftClick)) {
				TryBreakBlock((BlockPos)GetWorldPointerPos());
			}
		}
		private void OnRightClick() {
			TryUseItem(GetMainHandStack(), ItemActionTrigger.RightClick);

			// provisory
			level.InteractBlock((BlockPos)GetWorldPointerPos());
		}

		private void OnLeftHold() {
			ItemStack? mainHandStack = GetMainHandStack();

			if (!TryUseItem(mainHandStack, ItemActionTrigger.LeftClick)) {
				TryBreakBlock((BlockPos)GetWorldPointerPos());
			}
		}
		private void OnRightHold() {
			TryUseItem(GetMainHandStack(), ItemActionTrigger.RightHold);
		}

		private void OnLeftRelease() {
			TryUseItem(GetMainHandStack(), ItemActionTrigger.LeftRelease);			
		}
		private void OnRightRelease() {
			TryUseItem(GetMainHandStack(), ItemActionTrigger.RightRelease);
		}

		private bool TryUseItem(ItemStack? itemStack, ItemActionTrigger trigger) {
			Item? item = itemStack?.item;
			if (item == null) return false;

			if (item is not IItemActionHandler action) return false;
			if (!action.ValidateTrigger(trigger)) return false;

			ItemActionContext context = new() {
				itemStack = itemStack,
				player = this,
				level = level,
				trigger = trigger
			};

			if (!action.CanExecute(itemStack, context)) return false;

			return action.TryExecute(itemStack, context);
		}

		private bool TryBreakBlock(BlockPos blockPos) {
			if (!IsInBlockReach((Vector2)blockPos)) return false;

			BlockState blockState = level.GetBlockState(blockPos) ?? Blocks.air.defaultState;
			if (blockState.block == Blocks.air) return false;

			int itemBreakLevel = GetMainHandItemBreakLevel();
			int minBreakLevel = blockState.block.minBreakLevel;
			if (itemBreakLevel < minBreakLevel) return false;

			level.SetBlockState(blockPos, Blocks.air.defaultState);
			return true;
		}

		private int GetMainHandItemBreakLevel() {
			ItemStack? mainHandStack = GetMainHandStack();
			Item? item = mainHandStack?.item;

			if (item == null) return 0;

			if (item is IBlockBreakerItem breaker) {
				return breaker.GetBreakLevel(mainHandStack);
			}
			return -1;
		}

		private void ThrowMainHandStack() {
			IItemSlot mainHandSlot = hotbar.GetSlot(hotbar.GetMainSlotIndex());
			if (!mainHandSlot.HasStack()) return;

			ItemStack stack = mainHandSlot.GetStack()!;
			ItemEntity itemEntity = new(this, 2f, stack, transform.GetPos());
			level.AddEntity(itemEntity);

			mainHandSlot.SetStack(null);
		}

		public bool TryAddItemStack(ItemStack itemStack) {
			if (inventory.TryAddStack(itemStack)) return true;
			return hotbar.TryAddStack(itemStack);
		}

		public bool CanPlaceBlockAt(BlockPos blockPos) {
			Vector2 worldPos = (Vector2)blockPos;
			return IsInBlockReach(worldPos)
				   && level?.GetBlock(blockPos) == Blocks.air;
		}

		public bool CanBreakBlockAt(BlockPos blockPos) {
			Vector2 worldPos = (Vector2)blockPos;
			return IsInBlockReach(worldPos)
				   && level?.GetBlock(blockPos) != Blocks.air;
		}

		public bool IsInBlockReach(Vector2 worldPos) {
			float dist = Vector2.Distance(worldPos, GetCenter());
			return dist <= MAX_BLOCK_REACH 
				&& !level.GetTilesCovered(playerTransform.Collider.bounds)
						 .Contains((BlockPos)worldPos);
		}

		public Inventory GetInventory() => inventory;
		public Hotbar GetHotbar() => hotbar;

		public ItemStack? GetMainHandStack() {
			return hotbar.GetSlot(hotbar.GetMainSlotIndex()).GetStack();
		}

		public Vector2 GetCenter() => playerTransform.Collider.bounds.center;

		public bool IsHoldingLeftClick() => isHoldingLeftClick;
		public bool IsHoldingRightClick() => isHoldingRightClick;

		public Vector2 GetScreenPointerPos() => screenPointerPos;
		public Vector2 GetWorldPointerPos() {
			Vector3 screenPos = screenPointerPos;

			Canvas canvas = Soulbound.instance.GetUIHandler().GetCanvas();
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

		public override void SetPos(Vector2 pos) {
			if (transform != null) transform.SetPos(pos);
			else initialPos = pos;
		}
		public override Vector2 GetPos() => transform?.GetPos() ?? initialPos;
	}
}
