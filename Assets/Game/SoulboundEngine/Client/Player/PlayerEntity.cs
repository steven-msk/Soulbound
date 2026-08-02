using SoulboundEngine.Client.Input;
using SoulboundEngine.Client.Interaction;
using SoulboundEngine.Client.Item;
using SoulboundEngine.Client.Item.Container;
using SoulboundEngine.Client.UI.Screen;
using SoulboundEngine.Client.World.Block;
using SoulboundEngine.Client.World.Block.State;
using SoulboundEngine.Client.World.Entity;
using SoulboundEngine.Client.World.Level;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.Player {
	using Item = Item.Item;
	using Logger = Debug.Logging.Logger;

	public class PlayerEntity : Entity, IInputEventHandler {
		public static readonly EntityDescriptor<PlayerEntity> DESCRIPTOR = EntityDescriptor.Of<PlayerEntity>((_, level) => throw new InvalidOperationException());
		const float MAX_BLOCK_REACH = 5f;
		private readonly SoulboundClient client;
		private readonly PlayerInventory inventory;
		private bool isInventoryOpen;
		private Vector2 screenPointerPos;
		private bool isHoldingLeftClick;
		private bool isHoldingRightClick;
		private bool isHoldingCtrl;
		private InventoryScreenHandler? activeInventoryScreenHandler;
		private IScreenHandle? activeInventoryScreen;
		private new readonly PlayerTransformAdapter transformAdapter;
		private TransitStackHandler? transitStack;

		public PlayerEntity(SoulboundClient client, Level level)
			: base(DESCRIPTOR, level) {
			this.client = client;
			this.transformAdapter = new PlayerTransformAdapter(this);
			this.inventory = new PlayerInventory();
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

		// temporary hook from WorldRenderer
		public void FrameUpdate() {
			if (this.isHoldingLeftClick) this.OnLeftHold();
			if (this.isHoldingRightClick) this.OnRightHold();

			if (this.activeInventoryScreenHandler != null) {
				if (!this.activeInventoryScreenHandler.CanUse(this)) {
					this.CloseInventoryScreen();
				}
			}
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
			this.activeInventoryScreenHandler!.OnClosed(this);

			this.activeInventoryScreen = null;
			this.activeInventoryScreenHandler = null;
			this.isInventoryOpen = false;
		}

		private void OnLeftClick() {
			if (!this.Interact(ItemStack.OnPrimaryUseOnEntity, ItemStack.OnPrimaryUseOnBlock, ItemStack.OnPrimaryUse)) {
				this.TryBreakBlock((BlockPos)this.GetWorldPointerPos());
			}
		}
		private void OnRightClick() {
			this.Interact(ItemStack.OnSecondaryUseOnEntity, ItemStack.OnSecondaryUseOnBlock, ItemStack.OnSecondaryUse);
		}

		[Obsolete]
		private void OnLeftHold() {
		}
		[Obsolete]
		private void OnRightHold() {
		}

		[Obsolete]
		private void OnLeftRelease() {
		}
		[Obsolete]
		private void OnRightRelease() {
		}

		private bool Interact(
				Func<ItemStack, PlayerEntity, Entity, IActionResult> entityUse,
				Func<ItemStack, BlockInteractionResult, IActionResult> blockUse,
				Func<ItemStack, Level, PlayerEntity, BlockPos, IActionResult> normalUse
			) {
			Vector2 interactionPoint = this.GetWorldPointerPos();
			ItemStack stack = this.GetMainHandStack();
			return Interact(interactionPoint, stack, this, entityUse, blockUse, normalUse);
		}

		private static bool Interact(
				Vector2 interactionPoint, 
				ItemStack stack, 
				PlayerEntity player,
				Func<ItemStack, PlayerEntity, Entity, IActionResult> entityUse,
				Func<ItemStack, BlockInteractionResult, IActionResult> blockUse,
				Func<ItemStack, Level, PlayerEntity, BlockPos, IActionResult> normalUse
			) {
			if (stack.IsEmpty()) return false;

			if (player.CanInteractWithEntityAt(interactionPoint, out Entity targetEntity)) {
				IActionResult actionResult = entityUse(stack, player, targetEntity);
				if (HandleActionResult(actionResult, player)) return true;
			}

			if (player.CanInteractWithBlockAt(interactionPoint, out BlockState blockState, out BlockPos blockPos)) {
				BlockInteractionResult blockInteractionResult = new(player.level, blockPos, blockState, stack, player);
				IActionResult actionResult = blockUse(stack, blockInteractionResult);
				if (HandleActionResult(actionResult, player)) return true;
			}

			blockPos = (BlockPos)interactionPoint;
			IActionResult result = normalUse(stack, player.level, player, blockPos);
			return HandleActionResult(result, player);
		}

		private static bool HandleActionResult(IActionResult result, PlayerEntity player) {
			if (result is IActionResult.Success success) {
				ItemStack? newHandStack = success.itemContext.newHandStack;
				if (newHandStack is { } stack) {
					player.SetMainHandStack(stack);
				}
				return true;
			} else if (result is IActionResult.Fail) {
				return true;
			}
			return false;
		}

		public bool CanInteractWithEntityAt(Vector2 pos, out Entity entity) {
			if (this.level.TryGetEntityAt(pos, out entity)) {
				if (!this.IsInBlockReach(pos)) return false;
			}
			return true;
		}

		public bool CanInteractWithBlockAt(Vector2 pos, out BlockState blockState, out BlockPos blockPos) {
			blockPos = (BlockPos)pos;
			BlockState? state = this.level.GetBlockState(blockPos);
			if (state == null) {
				blockState = null!;
				return false;
			}

			blockState = state;
			if (blockState == Blocks.AIR.DefaultState) return false;
			return this.IsInBlockReach(blockPos.GetCenter());
		}

		private bool TryBreakBlock(BlockPos blockPos) {
			if (!this.IsInBlockReach((Vector2)blockPos)) return false;

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

		// temporary loot table implementation
		public float GetLuck() => 0f;

		public ItemStack GetMainHandStack() {
			ItemStack transitStack = this.transitStack?.GetStack() ?? ItemStack.EMPTY;
			return transitStack.IsEmpty() ? this.inventory.GetMainStack() : transitStack;
		}

		// TODO: implement PlayerEntity.SetMainHandStack
		// this requires transit stack sync which isnt supported yet
		// calling transitStack.SetStack right now will desync inventory screen handlers' transit stack
		public void SetMainHandStack(ItemStack newStack) {
			Logger.LogInfo("set main hand stack: {}", newStack);
		}

		public bool IsHoldingLeftClick() => this.isHoldingLeftClick;
		public bool IsHoldingRightClick() => this.isHoldingRightClick;

		public Vector2 GetScreenPointerPos() => this.screenPointerPos;
		public Vector2 GetWorldPointerPos() {
			// still "depends" on Unity internals, just hidden behind the client layer
			// keep in mind PlayerEntity is at core layer, independent of UnityEngine
			return this.client.ScreenToWorldPoint(this.screenPointerPos);
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
