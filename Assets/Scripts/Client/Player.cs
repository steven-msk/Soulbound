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

		public ItemStack? MainHandStack { get; private set; }

		private int mainHandIndex;
		const float MAX_BLOCK_REACH = 5f;

		private Vector2 screenPointerPos;

		private bool isHoldingLeftClick;
		private bool isHoldingRightClick;

		public Player(Vector2 initialPos)
			: base(DESCRIPTOR, initialPos) {
			inventory = new Inventory();
			hotbar = new Hotbar();
			hotbar.mainSlotChanged += (oldIndex, newIndex) => {
				IItemSlot oldSlot = hotbar.GetSlot(oldIndex);
				oldSlot.stackChanged -= MainHandStackChanged;
				oldSlot.quantityChanged -= MainHandQuantityChanged;

				IItemSlot newSlot = hotbar.GetSlot(newIndex);
				newSlot.stackChanged += MainHandStackChanged;
				newSlot.quantityChanged += MainHandQuantityChanged;

				mainHandIndex = newIndex;
				MainHandStackChanged(oldSlot.GetStack(), newSlot.GetStack());
			};
			this.initialPos = initialPos;
			Soulbound.instance.GetInputManager().PushContext(this);

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
			playerTransform.SetPos(initialPos);

			mainHandIndex = hotbar.GetMainSlotIndex();
			IItemSlot mainHandSlot = hotbar.GetSlot(mainHandIndex);
			mainHandSlot.stackChanged += MainHandStackChanged;
			mainHandSlot.quantityChanged += MainHandQuantityChanged;

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
			TryUseItem(GetMainHandStack(), ItemActionTrigger.LeftClick);
		}
		private void OnRightClick() {
			TryUseItem(GetMainHandStack(), ItemActionTrigger.RightClick);

			// provisory
			level.InteractBlock((BlockPos)GetWorldPointerPos());
		}

		private void OnLeftHold() {
			TryUseItem(GetMainHandStack(), ItemActionTrigger.LeftHold);
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
			return hotbar.GetSlot(mainHandIndex).GetStack();
		}
		private void MainHandStackChanged(ItemStack? oldStack, ItemStack? newStack) {

		}
		private void MainHandQuantityChanged(ItemStack itemStack, int oldQuantity, int newQuantity) {

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
