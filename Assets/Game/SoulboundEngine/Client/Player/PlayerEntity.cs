using SoulboundEngine.Client.Input;
using SoulboundEngine.Client.Interaction;
using SoulboundEngine.Client.Item;
using SoulboundEngine.Client.Item.Container;
using SoulboundEngine.Client.UI.Screen;
using SoulboundEngine.Client.World.Block;
using SoulboundEngine.Client.World.Block.State;
using SoulboundEngine.Client.World.Entity;
using SoulboundEngine.Client.World.Level;
using SoulboundEngine.Core.Event;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.Player {
	using Item = Item.Item;

	public class PlayerEntity : Entity, IInputEventHandler, IInteractionHandler<ItemInteraction>, IInteractionHandler<BlockInteraction> {
		public static readonly EntityDescriptor<PlayerEntity> DESCRIPTOR = EntityDescriptor.Of<PlayerEntity>((_, level) => throw new InvalidOperationException());
		const float MAX_BLOCK_REACH = 5f;
		private readonly SoulboundClient client;
		private readonly PlayerInventory inventory;
		private bool isInventoryOpen;
		private readonly InteractionResolver interactionResolver;
		private Vector2 screenPointerPos;
		private bool isHoldingLeftClick;
		private bool isHoldingRightClick;
		private bool isHoldingCtrl;
		private InventoryScreenHandler? activeInventoryScreenHandler;
		private IScreenHandle? activeInventoryScreen;
		private new readonly PlayerTransformAdapter transformAdapter;
		private TransitStackHandler? transitStack;

		// provisory guard for not breaking the block instantly after it was placed
		// TODO: fix gameplay input overlaps
		private bool leftClickBlockBreakGuard;

		public PlayerEntity(SoulboundClient client, Level level)
			: base(DESCRIPTOR, level) {
			this.client = client;
			this.transformAdapter = new PlayerTransformAdapter(this);
			this.inventory = new PlayerInventory();
			this.interactionResolver = new InteractionResolver();

			this.interactionResolver.RegisterHandler<ItemInteraction>(this);
			this.interactionResolver.RegisterHandler<BlockInteraction>(this);
		}

		public bool isJumping { get; private set; }

		IEnumerable<InputEventListener> IInputEventHandler.GetListeners() {
			return new InputEventListener[] {
				InputEventListener.ConsumePerformed(InputTokens.Player.changeHotbarSlot, inputEvent => {
					int slotIndex = int.Parse(inputEvent.context.control.name) - 1;
					this.inventory.SetMainSlot(slotIndex);
				}),
				InputEventListener.ConsumePerformed(InputTokens.Player.scrollHotbarSlot, inputEvent => {
					float scrollDelta = inputEvent.context.ReadValue<float>();
					int nextSlot = this.inventory.GetMainSlot() - (int)scrollDelta;

					if (nextSlot < 0) nextSlot += PlayerInventory.HOTBAR_SIZE;
					nextSlot %= PlayerInventory.HOTBAR_SIZE;
					this.inventory.SetMainSlot(nextSlot);
				}),
				InputEventListener.ObserveAny(InputTokens.Mouse.position, inputEvent => {
					this.screenPointerPos = inputEvent.context.ReadValue<Vector2>();
				}),
				new(InputTokens.Mouse.leftClick, InputEvent.Phase.Performed | InputEvent.Phase.Canceled, inputEvent => {
					if (inputEvent.phase == InputEvent.Phase.Performed) {
						this.OnLeftClick();
						this.isHoldingLeftClick = true;
					} else if (inputEvent.phase == InputEvent.Phase.Canceled) {
						this.OnLeftRelease();
						this.isHoldingLeftClick = false;
					}
					return InputHandleResult.Consume;
				}),
				new(InputTokens.Mouse.rightClick, InputEvent.Phase.Performed | InputEvent.Phase.Canceled, inputEvent => {
					if (inputEvent.phase == InputEvent.Phase.Performed) {
						this.OnRightClick();
						this.isHoldingRightClick = true;
					} else if (inputEvent.phase == InputEvent.Phase.Canceled) {
						this.OnRightRelease();
						this.isHoldingRightClick = false;
					}
					return InputHandleResult.Consume;
				}),
				new(InputTokens.Player.move, InputEvent.Phase.Performed | InputEvent.Phase.Canceled, inputEvent => {
					this.SetNormalVelocityX(
						inputEvent.phase == InputEvent.Phase.Performed
							? inputEvent.context.ReadValue<Vector2>().x
							: 0f
					);
					return InputHandleResult.Consume;
				}),
				new(InputTokens.Player.jump, InputEvent.Phase.Performed | InputEvent.Phase.Canceled, inputEvent => {
					this.SetJumping(inputEvent.phase == InputEvent.Phase.Performed);
					return InputHandleResult.Consume;
				}),
				InputEventListener.ConsumePerformed(InputTokens.Keyboard.Q, _ => this.ThrowFromMainHand(this.isHoldingCtrl)),
				new(InputTokens.Keyboard.CTRL, InputEvent.Phase.Performed | InputEvent.Phase.Canceled, inputEvent => {
					this.isHoldingCtrl = inputEvent.phase == InputEvent.Phase.Performed;
					return InputHandleResult.Consume;
				}),
				InputEventListener.ConsumePerformed(InputTokens.Player.toggleInventory, _ => {
					if (!this.isInventoryOpen) {
						this.OpenInventoryScreen(new DelegatedInventoryScreenHandlerFactory(
							(inventory, _) => {
								InventoryScreenHandlerContext context = InventoryScreenHandlerContext.Of(this.client, (BlockPos)this.GetPosition(), this.level);
								return new PlayerInventoryScreenHandler(InventoryScreenHandlerType.PLAYER_INVENTORY, inventory, context);
							}
						));
					} else {
						this.CloseInventoryScreen();
					}
				})
			};
		}

		public void SetJumping(bool jumping) {
			this.isJumping = jumping;
			this.transformAdapter.SetJumping(jumping);
		}

		public void StopHorizontalMovement() {
			this.SetNormalVelocityX(0f);
		}

		public override void FrameUpdate() {
			base.FrameUpdate();
			if (this.isHoldingLeftClick) this.OnLeftHold();
			if (this.isHoldingRightClick) this.OnRightHold();

			if (this.activeInventoryScreenHandler != null) {
				if (!this.activeInventoryScreenHandler.CanUse(this)) {
					this.CloseInventoryScreen();
				}
			}
		}

		private void OnLeftClick() {
			if (!this.ResolveItemOrBlockInteraction(InteractionTrigger.LeftClick)) {

				// PROTOTYPICAL
				BlockPos blockPos = (BlockPos)this.GetWorldPointerPos();
				if (this.TryBreakBlock(blockPos)) {
					EventBus.Publish(new BlockBrokenEvent(blockPos, this.level));
				}
			}
		}
		private void OnRightClick() {
			this.ResolveItemOrBlockInteraction(InteractionTrigger.RightClick);
		}

		private void OnLeftHold() {
			if (!this.ResolveItemOrBlockInteraction(InteractionTrigger.LeftHold)) {

				// PROTOTYPICAL
				BlockPos blockPos = (BlockPos)this.GetWorldPointerPos();
				if (this.TryBreakBlock(blockPos)) {
					EventBus.Publish(new BlockBrokenEvent(blockPos, this.level));
				}
			}
		}
		private void OnRightHold() {
			this.ResolveItemOrBlockInteraction(InteractionTrigger.RightHold);
		}

		private void OnLeftRelease() {
			this.ResolveItemOrBlockInteraction(InteractionTrigger.LeftRelease);
			this.leftClickBlockBreakGuard = false;
		}
		private void OnRightRelease() {
			this.ResolveItemOrBlockInteraction(InteractionTrigger.RightRelease);
		}

		public void OpenInventoryScreen(IInventoryScreenHandlerFactory handlerFactory) {
			if (this.activeInventoryScreen != null) return;
			InventoryScreenHandler handler = handlerFactory.Create(this.inventory, this);
			this.activeInventoryScreenHandler = handler;
			this.activeInventoryScreen = InventoryScreens.Open(handler, this.client, this.inventory, this);
			this.isInventoryOpen = true;
		}

		public void CloseInventoryScreen() {
			if (this.activeInventoryScreen == null) return;
			this.client.CloseScreen(this.activeInventoryScreen);
			this.activeInventoryScreen = null;
			this.activeInventoryScreenHandler = null;
			this.isInventoryOpen = false;
		}

		private bool ResolveItemOrBlockInteraction(InteractionTrigger trigger) {
			ItemInteraction itemInteraction = this.GetItemInteraction(trigger);
			if (this.interactionResolver.Resolve(itemInteraction)) {
				this.leftClickBlockBreakGuard = itemInteraction.itemStack.item is IPlaceableItem;
				return true;
			}
			return this.interactionResolver.Resolve(this.GetBlockInteraction(trigger));
		}

		private ItemInteraction GetItemInteraction(InteractionTrigger trigger) {
			return new ItemInteraction {
				itemStack = this.GetMainHandStack(),
				player = this,
				level = this.level,
				trigger = trigger
			};
		}

		private BlockInteraction GetBlockInteraction(InteractionTrigger trigger) {
			BlockPos blockPos = (BlockPos)this.GetWorldPointerPos();
			return new BlockInteraction {
				trigger = trigger,
				blockPos = blockPos,
				blockState = this.level.GetBlockState(blockPos),
				itemStack = this.GetMainHandStack(),
				level = this.level,
				player = this
			};
		}

		// TODO: rework interaction design
		
		// provisory priority
		int IInteractionHandler<ItemInteraction>.priority => 0;

		bool IInteractionHandler<ItemInteraction>.CanHandle(in ItemInteraction ctx) {
			Item item = ctx.itemStack.item;
			if (ctx.itemStack.IsEmpty()) return false;
			if (item is not IInteractableItem interactable) return false;

			if (!interactable.ValidateTrigger(ctx.trigger)) return false;

			return interactable.CanExecute(in ctx.itemStack, in ctx);
		}

		bool IInteractionHandler<ItemInteraction>.Handle(in ItemInteraction ctx) {
			ItemStack stack = ctx.itemStack;
			IInteractableItem interactable = (IInteractableItem)stack.item;
			return interactable.TryExecute(ref stack, in ctx);
		}

		int IInteractionHandler<BlockInteraction>.priority => 0;

		bool IInteractionHandler<BlockInteraction>.CanHandle(in BlockInteraction ctx) {

			// interaction handler shouldnt guard block interactions only inside the player reach
			// some blocks may be interactable even if theyre out of reach, though this is a false assumption for pre-prod
			// CanInteract will need to explicitly check if the player is in range if it requires it
			// for this case the handler is implemented in Player so the this CanHandle guards it
			// but keep this in mind for future implementations
			bool isInReach = this.IsInBlockReach((Vector2)ctx.blockPos);
			if (!isInReach) return false;

			if (ctx.blockState.block is not IInteractableBlock interactable) return false;

			if (!interactable.ValidateTrigger(ctx.trigger)) return false;

			return interactable.CanInteract(in ctx);
		}

		bool IInteractionHandler<BlockInteraction>.Handle(in BlockInteraction ctx) {
			IInteractableBlock interactable = (IInteractableBlock)ctx.blockState.block;
			interactable.OnInteract(in ctx);
			return true;
		}

		private bool TryBreakBlock(BlockPos blockPos) {
			if (!this.IsInBlockReach((Vector2)blockPos) || this.leftClickBlockBreakGuard) return false;

			BlockState blockState = this.level.GetBlockState(blockPos) ?? Blocks.AIR.DefaultState;
			if (blockState.block == Blocks.AIR) return false;

			int itemBreakLevel = this.GetMainHandItemBreakLevel();
			int minBreakLevel = blockState.block.minBreakLevel;
			if (itemBreakLevel < minBreakLevel) return false;

			this.level.SetBlockState(blockPos, Blocks.AIR.DefaultState);
			Block.DropStacks(blockState, this.level, blockPos, null);
			return true;
		}

		private int GetMainHandItemBreakLevel() {
			ItemStack mainHandStack = this.GetMainHandStack();
			Item item = mainHandStack.item;

			if (mainHandStack.IsEmpty()) return 0;

			if (item is IBlockBreakerItem breaker) {
				return breaker.GetBreakLevel(mainHandStack);
			}
			return -1;
		}

		private void ThrowFromMainHand(bool ctrl) {
			ItemStack mainHandStack = this.GetMainHandStack();
			if (mainHandStack.IsEmpty()) return;

			int throwAmount = ctrl ? mainHandStack.count : 1;
			ItemStack thrownStack = mainHandStack.CopyWithCount(throwAmount);
			mainHandStack.Decrement(throwAmount);

			this.DropStack(this.level, thrownStack);
		}

		public bool TryAddItemStack(ItemStack itemStack) {
			bool consumed = this.inventory.TryAddStack(ref itemStack);
			this.activeInventoryScreenHandler?.OnContentChanged(this.inventory);
			return consumed;
		}

		public bool CanPlaceBlockAt(BlockPos blockPos) {
			Vector2 worldPos = (Vector2)blockPos;
			return this.IsInBlockReach(worldPos)
				   && this.level?.GetBlock(blockPos) == Blocks.AIR;
		}

		public bool CanBreakBlockAt(BlockPos blockPos) {
			Vector2 worldPos = (Vector2)blockPos;
			return this.IsInBlockReach(worldPos)
				   && this.level?.GetBlock(blockPos) != Blocks.AIR;
		}

		public bool IsInBlockReach(Vector2 worldPos) {
			float dist = Vector2.Distance(worldPos, this.GetCenter());
			return dist <= MAX_BLOCK_REACH 
				&& !this.level.GetTilesCovered(this.GetBoundingBox())
						 .Contains((BlockPos)worldPos);
		}

		public PlayerInventory GetInventory() => this.inventory;

		public ItemStack GetMainHandStack() {
			ItemStack transitStack = this.transitStack?.GetStack() ?? ItemStack.EMPTY;
			return transitStack.IsEmpty() ? this.inventory.GetMainStack() : transitStack;
		}

		public bool IsHoldingLeftClick() => this.isHoldingLeftClick;
		public bool IsHoldingRightClick() => this.isHoldingRightClick;

		public Vector2 GetScreenPointerPos() => this.screenPointerPos;
		public Vector2 GetWorldPointerPos() {
			Vector3 screenPos = this.screenPointerPos;

			//Canvas canvas = SoulboundClient.Instance.UIHandler.GetCanvas();
			//RectTransform rootTransform = canvas.GetComponent<RectTransform>();
			//bool inWorldPoint = RectTransformUtility.ScreenPointToWorldPointInRectangle(
			//	rootTransform,
			//	screenPos,
			//	Camera.main,
			//	out var worldPoint
			//);
			//if (inWorldPoint) return worldPoint;

			screenPos.z = -Camera.main.transform.position.z;
			return Camera.main.ScreenToWorldPoint(screenPos);
		}

		public void SetTransitStackSource(TransitStackHandler transitStack) {
			this.transitStack = transitStack;
		}

		public void SetTransformHandle(IPlayerTransformHandle playerTransformHandle) {
			this.transformAdapter.SetHandle(playerTransformHandle);
		}

		private sealed class PlayerTransformAdapter : TransformAdapter {
			private IPlayerTransformHandle? transformHandle;

			public PlayerTransformAdapter(PlayerEntity player)
				: base(player) {
			}

			public void SetHandle(IPlayerTransformHandle? handle) {
				this.transformHandle = handle;
			}

			public void SetJumping(bool jumping) {
				this.transformHandle?.SetJumping(jumping);
			}
		}

		public interface IPlayerTransformHandle {
			void SetJumping(bool jumping);
		}
	}
}
