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
		private ActiveUseContext? activeItemUse;

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

		public override void Tick() {
			this.CheckItemUse();
			if (this.isHoldingLeftClick) this.OnLeftHoldTick();
			if (this.isHoldingRightClick) this.OnRightHoldTick();

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
			this.PrimaryInteract();
		}
		private void OnRightClick() {
			this.SecondaryInteract();
		}

		private void OnLeftHoldTick() {
			this.HandleInteractTick(InteractionType.Primary);
		}
		private void OnRightHoldTick() {
			this.HandleInteractTick(InteractionType.Secondary);
		}

		[Obsolete]
		private void OnLeftRelease() {
		}
		[Obsolete]
		private void OnRightRelease() {
		}

		private void HandleInteractTick(InteractionType type) {
			Action getInteractAction(InteractionType type) {
				return type switch {
					InteractionType.Primary => this.PrimaryInteract,
					InteractionType.Secondary => this.SecondaryInteract,
					_ => throw new ArgumentException()
				};
			}
			BlockPos blockPos = (BlockPos)this.GetWorldPointerPos();
			if (this.activeItemUse?.type == type) this.HandleUseTick();
			else if (this.activeItemUse == null && this.GetMainHandStack().ShouldContinueUse(type, this.level, this, blockPos)) {
				getInteractAction(type)();
			}
		}

		private void PrimaryInteract() {
			if (!this.Interact(InteractionType.Primary,
					ItemStack.OnPrimaryUseOnEntity, ItemStack.OnPrimaryUseOnBlock, ItemStack.OnPrimaryUse,
					AbstractBlock.AbstractBlockState.OnPrimaryUse, AbstractBlock.AbstractBlockState.OnPrimaryUseWithItem
				)) {
				this.TryBreakBlock((BlockPos)this.GetWorldPointerPos());
			}
		}

		private void SecondaryInteract() {
			this.Interact(InteractionType.Secondary,
				ItemStack.OnSecondaryUseOnEntity, ItemStack.OnSecondaryUseOnBlock, ItemStack.OnSecondaryUse,
				AbstractBlock.AbstractBlockState.OnSecondaryUse, AbstractBlock.AbstractBlockState.OnSecondaryUseWithItem
			);
		}

		private void HandleUseTick() {
			if (this.activeItemUse == null) return;
			this.activeItemUse = this.activeItemUse.Tick(finishedStack => {
				this.SetMainHandStackInternal(finishedStack);
				return null;
			}, this.SetMainHandStackInternal);
		}

		public void CancelItemUse() {
			this.activeItemUse?.Cancel(this.SetMainHandStackInternal);
			this.activeItemUse = null;
		}

		private void CheckItemUse() {
			if (!this.IsUsingItem()) return;
			if ((this.activeItemUse!.type == InteractionType.Primary && !this.isHoldingLeftClick)
					|| (this.activeItemUse!.type == InteractionType.Secondary && !this.isHoldingRightClick)) {
				this.CancelItemUse();
			}
		}

		private bool Interact(
				InteractionType type,
				Func<ItemStack, PlayerEntity, Entity, IActionResult> itemOnEntity,
				Func<ItemStack, BlockInteractionResult, IActionResult> itemOnBlock,
				Func<ItemStack, Level, PlayerEntity, BlockPos, IActionResult> itemInAir,
				Func<BlockState, Level, PlayerEntity, BlockPos, IActionResult> blockUse,
				Func<BlockState, ItemStack, Level, PlayerEntity, BlockPos, IActionResult> blockUseWithItem
			) {
			this.CancelItemUse();

			Vector2 interactionPoint = this.GetWorldPointerPos();
			ItemStack stack = this.GetMainHandStack();

			bool itemInteracted = ItemInteract(interactionPoint, stack, this, itemOnEntity, itemOnBlock, itemInAir);
			if (itemInteracted) {
				ItemStack usedStack = stack.OnItemUsed(type, this.level, this);
				this.SetMainHandStack(usedStack);

				int useTime = usedStack.GetUseTime(type, this.level, this);
				if (useTime > 0) {
					ActiveUseContext useContext = new(usedStack, type, this.level, this, useTime, useTime);
					this.activeItemUse = useContext;
				}
				return true;
			}

			BlockPos blockPos = (BlockPos)interactionPoint;
			BlockState? blockState = this.level.GetBlockState(blockPos);
			if (blockState == null) return false;
			return BlockInteract(blockState, blockPos, stack, this, blockUse, blockUseWithItem);
		}

		private static bool ItemInteract(
				Vector2 interactionPoint, 
				ItemStack stack, 
				PlayerEntity player,
				Func<ItemStack, PlayerEntity, Entity, IActionResult> onEntity,
				Func<ItemStack, BlockInteractionResult, IActionResult> onBlock,
				Func<ItemStack, Level, PlayerEntity, BlockPos, IActionResult> inAir
			) {
			if (stack.IsEmpty()) return false;

			if (player.CanInteractWithEntityAt(interactionPoint, out Entity targetEntity)) {
				IActionResult actionResult = onEntity(stack, player, targetEntity);
				if (actionResult is IActionResult.PassToBlockAction) return false;
				if (HandleActionResult(actionResult, player)) return true;
			}
			Logger.LogInfo(stack);

			if (player.CanInteractWithBlockAt(interactionPoint, out BlockState blockState, out BlockPos blockPos)) {
				BlockInteractionResult blockInteractionResult = new(player.level, blockPos, blockState, stack, player);
				IActionResult actionResult = onBlock(stack, blockInteractionResult);
				if (actionResult is IActionResult.PassToBlockAction) return false;
				if (HandleActionResult(actionResult, player)) return true;
			}

			blockPos = (BlockPos)interactionPoint;
			IActionResult result = inAir(stack, player.level, player, blockPos);
			if (result is IActionResult.PassToBlockAction) return false;
			return HandleActionResult(result, player);
		}

		private static bool BlockInteract(
			BlockState blockState, BlockPos blockPos, ItemStack stack, PlayerEntity player,
			Func<BlockState, Level, PlayerEntity, BlockPos, IActionResult> normalUse,
			Func<BlockState, ItemStack, Level, PlayerEntity, BlockPos, IActionResult> withItem
		) {
			if (!blockState.IsInteractable(player.level, blockPos)) return false;
			if (!player.IsInBlockReach(blockPos.GetCenter())) return false;

			if (!stack.IsEmpty()) {
				IActionResult actionResult = withItem(blockState, stack, player.level, player, blockPos);
				if (HandleActionResult(actionResult, player)) return true;
			}

			IActionResult result = normalUse(blockState, player.level, player, blockPos);
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
			if (!this.IsInBlockReach(pos)) {
				entity = null!;
				return false;
			}
			return this.level.TryGetEntityAt(pos, out entity);
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
			if (!Level.IsInBounds(blockPos)) return false;

			BlockState blockState = this.level.GetBlockState(blockPos) ?? Blocks.AIR.DefaultState;
			if (blockState.block == Blocks.AIR) return false;

			int itemBreakLevel = this.GetMainHandItemBreakLevel();
			int minBreakLevel = blockState.block.MinBreakLevel;
			if (itemBreakLevel < minBreakLevel) return false;

			this.level.SetBlockState(blockPos, blockState.block.OnBreak(this.level, blockPos, blockState, this));
			Block.DropStacks(blockState, this.level, blockPos, null);
			return true;
		}

		private int GetMainHandItemBreakLevel() {
			ItemStack mainHandStack = this.GetMainHandStack();
			return mainHandStack.GetBreakLevel();
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

		
		public void SetMainHandStack(ItemStack stack) {
			this.CancelItemUse();
			this.SetMainHandStackInternal(stack);
		}

		// TODO: implement PlayerEntity.SetMainHandStackInternal
		// this requires transit stack sync which isnt supported yet
		// calling transitStack.SetStack right now will desync inventory screen handlers' transit stack
		private void SetMainHandStackInternal(ItemStack stack) {
			if (ItemStack.AreEqual(stack, this.GetMainHandStack())) return;
			Logger.LogInfo("set main hand stack: {}", stack);
		}

		public bool IsHoldingLeftClick() => this.isHoldingLeftClick;
		public bool IsHoldingRightClick() => this.isHoldingRightClick;

		public Vector2 GetScreenPointerPos() => this.screenPointerPos;
		public Vector2 GetWorldPointerPos() {
			// still "depends" on Unity internals, just hidden behind the client layer
			// keep in mind PlayerEntity is at core layer, independent of UnityEngine
			return this.client.ScreenToWorldPoint(this.screenPointerPos);
		}

		public bool IsUsingItem() => this.activeItemUse != null;

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
