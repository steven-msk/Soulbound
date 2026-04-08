using SoulboundEngine.Client.Input;
using SoulboundEngine.Client.Interaction;
using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.ItemSystem.Container;
using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Client.World.EntitySystem;
using SoulboundEngine.Client.World.EntitySystem.Transform;
using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Common;
using SoulboundEngine.Core;
using SoulboundEngine.Core.Assets;
using SoulboundEngine.Core.Event;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

#nullable enable

namespace SoulboundEngine.Client.Players {
	public class Player : Entity, IInputContext, IInteractionHandler<ItemInteraction>, IInteractionHandler<BlockInteraction> {
		private static readonly AssetKey playerKey = new("player");
		private static readonly EntityDescriptor DESCRIPTOR = new((_, _) => throw new InvalidOperationException("Cannot create new player from factory"));
		private readonly Inventory inventory;
		private readonly Hotbar hotbar;
		private ITransitStackSource tranistStackSource = null!;
		private PlayerTransform playerTransform = null!;
		private new Vector2 initialPos;

		// TODO: interaction resolver shouldnt be created by the player
		private readonly InteractionResolver interactionResolver = new();

		const float MAX_BLOCK_REACH = 5f;

		private Vector2 screenPointerPos;
		private bool isHoldingLeftClick;
		private bool isHoldingRightClick;
		private bool isHoldingCtrl;

		// provisory guard for not breaking the block instantly after it was placed
		private bool leftClickBlockBreakGuard;

		public Player(Level level)
			: base(DESCRIPTOR, level) {
			inventory = new Inventory();
			hotbar = new Hotbar();

			interactionResolver.RegisterHandler<ItemInteraction>(this);
			interactionResolver.RegisterHandler<BlockInteraction>(this);
		}

		[Obsolete("Leaking Unity code in potential headless simulation code")]
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
				ThrowFromMainHand(isHoldingCtrl);
				return true;
			}
			if (inputEvent.token.Equals(InputTokens.Keyboard.CTRL)) {
				if (inputEvent.phase == InputActionPhase.Performed) {
					isHoldingCtrl = true;
				} else if (inputEvent.phase == InputActionPhase.Canceled) {
					isHoldingCtrl = false;
				}
			}
			return false;
		}

		public override void FrameUpdate() {
			base.FrameUpdate();
			if (isHoldingLeftClick) OnLeftHold();
			if (isHoldingRightClick) OnRightHold();
		}

		private void OnLeftClick() {
			if (!ResolveItemOrBlockInteraction(InteractionTrigger.LeftClick)) {

				// PROTOTYPICAL
				BlockPos blockPos = (BlockPos)GetWorldPointerPos();
				if (TryBreakBlock(blockPos)) {
					EventBus.Publish(new BlockBrokenEvent(blockPos, level));
				}
			}
		}
		private void OnRightClick() {
			ResolveItemOrBlockInteraction(InteractionTrigger.RightClick);
		}

		private void OnLeftHold() {
			if (!ResolveItemOrBlockInteraction(InteractionTrigger.LeftHold)) {

				// PROTOTYPICAL
				BlockPos blockPos = (BlockPos)GetWorldPointerPos();
				if (TryBreakBlock(blockPos)) {
					EventBus.Publish(new BlockBrokenEvent(blockPos, level));
				}
			}
		}
		private void OnRightHold() {
			ResolveItemOrBlockInteraction(InteractionTrigger.RightHold);
		}

		private void OnLeftRelease() {
			ResolveItemOrBlockInteraction(InteractionTrigger.LeftRelease);
			leftClickBlockBreakGuard = false;
		}
		private void OnRightRelease() {
			ResolveItemOrBlockInteraction(InteractionTrigger.RightRelease);
		}

		private bool ResolveItemOrBlockInteraction(InteractionTrigger trigger) {
			ItemInteraction itemInteraction = GetItemInteraction(trigger);
			if (interactionResolver.Resolve(itemInteraction)) {
				leftClickBlockBreakGuard = itemInteraction.itemStack?.item is IPlaceableItem;
				return true;
			}
			return interactionResolver.Resolve(GetBlockInteraction(trigger));
		}

		private ItemInteraction GetItemInteraction(InteractionTrigger trigger) {
			return new ItemInteraction {
				itemStack = GetMainHandStack(),
				player = this,
				level = this.level,
				trigger = trigger
			};
		}

		private BlockInteraction GetBlockInteraction(InteractionTrigger trigger) {
			BlockPos blockPos = (BlockPos)GetWorldPointerPos();
			return new BlockInteraction {
				trigger = trigger,
				blockPos = blockPos,
				blockState = this.level.GetBlockState(blockPos),
				itemStack = GetMainHandStack(),
				level = this.level
			};
		}

		// provisory priority
		int IInteractionHandler<ItemInteraction>.priority => 0;

		bool IInteractionHandler<ItemInteraction>.CanHandle(in ItemInteraction ctx) {
			Item? item = ctx.itemStack?.item;
			if (item is not IItemInteractionListener listener) return false;

			if (!listener.ValidateTrigger(ctx.trigger)) return false;

			return listener.CanExecute(ctx.itemStack, in ctx);
		}

		bool IInteractionHandler<ItemInteraction>.Handle(in ItemInteraction ctx) {
			ItemStack stack = ctx.itemStack;
			IItemInteractionListener listener = (IItemInteractionListener)stack.item;
			return listener.TryExecute(stack, in ctx);
		}

		int IInteractionHandler<BlockInteraction>.priority => 0;

		bool IInteractionHandler<BlockInteraction>.CanHandle(in BlockInteraction ctx) {

			// interaction handler shouldnt guard block interactions only inside the player reach
			// some blocks may be interactable even if theyre out of reach, though this is a false assumption for pre-prod
			// CanInteract will need to explicitly check if the player is in range if it requires it
			// for this case the handler is implemented in Player so the this CanHandle guards it
			// but keep this in mind for future implementations
			bool isInReach = IsInBlockReach((Vector2)ctx.blockPos);
			if (!isInReach) return false;

			if (ctx.blockState.block is not IBlockInteractionListener listener) return false;

			if (!listener.ValidateTrigger(ctx.trigger)) return false;

			return listener.CanInteract(in ctx);
		}

		bool IInteractionHandler<BlockInteraction>.Handle(in BlockInteraction ctx) {
			IBlockInteractionListener listener = (IBlockInteractionListener)ctx.blockState.block;
			listener.OnInteract(in ctx);
			return true;
		}

		private bool TryBreakBlock(BlockPos blockPos) {
			if (!IsInBlockReach((Vector2)blockPos) || leftClickBlockBreakGuard) return false;

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

		private void ThrowFromMainHand(bool ctrl) {
			ItemStack? mainHandStack = GetMainHandStack();
			if (mainHandStack == null) return;

			int throwAmount = ctrl ? mainHandStack.quantity : 1;
			ItemStack thrownStack = mainHandStack.Clone(throwAmount);
			mainHandStack.Decrement(throwAmount);

			ItemEntity itemEntity = new(this, pickupDelaySec: 2f, thrownStack, level);
			level.AddEntity(itemEntity);
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
			ItemStack? transitStack = tranistStackSource?.GetTransitStack();
			return transitStack ?? hotbar.GetSlot(hotbar.GetMainSlotIndex()).GetStack();
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

		public void SetTransitStackSource(ITransitStackSource source) {
			this.tranistStackSource = source;
		}
	}
}
