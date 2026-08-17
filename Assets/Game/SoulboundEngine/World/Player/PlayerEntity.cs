using SoulboundEngine.Client;
using SoulboundEngine.Client.UI.Screen;
using SoulboundEngine.Client.World.Widget;
using SoulboundEngine.Common.Math;
using SoulboundEngine.Interaction;
using SoulboundEngine.Item;
using SoulboundEngine.Item.Container;
using SoulboundEngine.World.Block;
using SoulboundEngine.World.Block.Entity;
using SoulboundEngine.World.Block.State;
using SoulboundEngine.World.Entity;
using SoulboundEngine.World.Physics;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.World.Player {
	using Block = Block.Block;
	using Entity = Entity.Entity;
	using Level = Level.Level;

	public class PlayerEntity : Entity {
		private const double MAX_BLOCK_REACH = 5d;
		private const double PICKUP_BOX_STRETCH_X = 1.5d;
		private const double PICKUP_BOX_STRETCH_Y = 1.0d;
		private const int DROP_PICKUP_DELAY = 75;
		private readonly SoulboundClient client;
		private readonly PlayerInventory inventory;
		private Vector2 screenPointerPos;
		private InventoryScreenHandler? activeInventoryScreenHandler;
		private IScreenHandle? activeInventoryScreen;
		private ActiveUseContext? activeItemUse;
		private BlockPos previousPointerBlockPos;
		private bool isHoldingLeft;
		private bool isHoldingRight;
		private bool isInventoryOpen;

		public PlayerEntity(SoulboundClient client, Level level)
			: base(EntityType.PLAYER, level) {
			this.client = client;
			this.inventory = new PlayerInventory();
		}

		public bool isJumping { get; private set; }
		public float movementX { get; private set; }

		public void SetJumping(bool jumping) {
			this.isJumping = jumping;
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

		public void ToggleInventory() {
			if (!this.isInventoryOpen) {
				this.OpenInventoryScreen(new DelegatedInventoryScreenHandlerFactory(
					(inventory, _) => {
						InventoryScreenHandlerContext context = InventoryScreenHandlerContext.Of(this.client, BlockPos.From(this.GetPosition()), this.level);
						return new PlayerInventoryScreenHandler(InventoryScreenHandlerType.PLAYER_INVENTORY, inventory, context);
					}
				));
			} else {
				this.CloseInventoryScreen();
			}
		}

		public override void Tick() {
			Vec2d movementInput = new(this.movementX, 0.0d);
			this.Travel(movementInput);

			this.DoBlockHover();
			this.CheckItemUse();
			if (this.isHoldingLeft) this.OnLeftHoldTick();
			if (this.isHoldingRight) this.OnRightHoldTick();

			if (this.activeInventoryScreenHandler != null && !this.activeInventoryScreenHandler.CanUse(this)) {
				this.CloseInventoryScreen();
			}

			List<Entity> collidedEntities = this.level.GetEntities(this, this.GetPickupArea(), ALL);
			foreach (Entity entity in collidedEntities) {
				this.Touch(entity);
			}
		}

		private AABB GetPickupArea() {
			return this.boundingBox.Stretch(PICKUP_BOX_STRETCH_X, PICKUP_BOX_STRETCH_Y);
		}

		private void Touch(Entity entity) {
			entity.PlayerTouch(this);
		}

		public override float GetSpeed() => 0.1f;

		protected override double GetGravity() => 0.981d / SharedConstants.TICKS_PER_SECOND;

		private void OnLeftHoldTick() {
			this.HandleInteractTick(InteractionType.Primary);
		}
		private void OnRightHoldTick() {
			this.HandleInteractTick(InteractionType.Secondary);
		}

		public void SetHoldingLeft(bool holding) => this.isHoldingLeft = holding;
		public void SetHoldingRight(bool holding) => this.isHoldingRight = holding;

		public void OnLeftClick() {
			this.PrimaryInteract();
		}

		public void OnRightClick() {
			this.SecondaryInteract();
		}

		private void DoBlockHover() {
			Vec2d pointerPos = this.GetWorldPointerPos();
			BlockPos pointerBlockPos = BlockPos.From(pointerPos);
			BlockState? currentState = this.level.GetBlockState(pointerBlockPos);

			if (pointerBlockPos == this.previousPointerBlockPos) {
				currentState?.OnHoverTick(this.GetMainHandStack(), this.level, this, pointerBlockPos);
			} else {
				BlockState? previousState = this.level.GetBlockState(this.previousPointerBlockPos);
				previousState?.OnHoverLeave(this.GetMainHandStack(), this.level, this, this.previousPointerBlockPos);
				currentState?.OnHoverEnter(this.GetMainHandStack(), this.level, this, pointerBlockPos);
			}
			this.previousPointerBlockPos = pointerBlockPos;
		}

		private void HandleInteractTick(InteractionType type) {
			Action getInteractAction(InteractionType type) {
				return type switch {
					InteractionType.Primary => this.PrimaryInteract,
					InteractionType.Secondary => this.SecondaryInteract,
					_ => throw new ArgumentException()
				};
			}
			BlockPos blockPos = BlockPos.From(this.GetWorldPointerPos());
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
				this.TryBreakBlock(BlockPos.From(this.GetWorldPointerPos()));
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
			if ((this.activeItemUse!.type == InteractionType.Primary && !this.isHoldingLeft)
					|| (this.activeItemUse!.type == InteractionType.Secondary && !this.isHoldingRight)) {
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

			Vec2d interactionPoint = this.GetWorldPointerPos();

			bool itemInteracted = ItemInteract(interactionPoint, this.GetMainHandStack(), this, itemOnEntity, itemOnBlock, itemInAir);
			if (itemInteracted) {
				ItemStack usedStack = this.GetMainHandStack().OnItemUsed(type, this.level, this);
				this.SetMainHandStackInternal(usedStack);

				int useTime = usedStack.GetUseTime(type, this.level, this);
				if (useTime > 0) {
					ActiveUseContext useContext = new(usedStack, type, this.level, this, useTime, useTime);
					this.activeItemUse = useContext;
				}
				return true;
			}

			BlockPos blockPos = BlockPos.From(interactionPoint);
			BlockState? blockState = this.level.GetBlockState(blockPos);
			if (blockState == null) return false;
			return BlockInteract(blockState, blockPos, this.GetMainHandStack(), this, blockUse, blockUseWithItem);
		}

		private static bool ItemInteract(
				Vec2d interactionPoint,
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

			if (player.CanInteractWithBlockAt(interactionPoint, out BlockState blockState, out BlockPos blockPos)) {
				BlockInteractionResult blockInteractionResult = new(player.level, blockPos, blockState, stack, player);
				IActionResult actionResult = onBlock(stack, blockInteractionResult);
				if (actionResult is IActionResult.PassToBlockAction) return false;
				if (HandleActionResult(actionResult, player)) return true;
			}

			blockPos = BlockPos.From(interactionPoint);
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
				IActionResult.ItemContext context = success.itemContext;
				ItemStack? newHandStack = context.newHandStack;
				ItemStack stack = newHandStack.GetValueOrDefault(player.GetMainHandStack()).Copy();
				if (context.damageItem) stack = stack.Damage(1);
				player.SetMainHandStack(stack);
				return true;
			} else if (result is IActionResult.Fail) {
				return true;
			}
			return false;
		}

		public bool CanInteractWithEntityAt(Vec2d pos, out Entity entity) {
			if (!this.IsInBlockReach(pos)) {
				entity = null!;
				return false;
			}
			return this.level.TryGetEntityAt(pos, out entity);
		}

		public bool CanInteractWithBlockAt(Vec2d pos, out BlockState blockState, out BlockPos blockPos) {
			blockPos = BlockPos.From(pos);
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
			if (!this.IsInBlockReach(blockPos.ToVec2d())) return false;
			if (!Level.IsInBounds(blockPos)) return false;

			BlockState blockState = this.level.GetBlockState(blockPos) ?? Blocks.AIR.DefaultState;
			if (blockState.block == Blocks.AIR) return false;

			ItemStack stack = this.GetMainHandStack();
			int itemBreakLevel = stack.GetBreakLevel();
			int minBreakLevel = blockState.block.MinBreakLevel;
			if (itemBreakLevel < minBreakLevel) return false;

			this.level.SetBlockState(blockPos, blockState.block.OnBreak(this.level, blockPos, blockState, this));
			Block.DropStacks(blockState, this.level, blockPos, null);
			stack.Damage(1);
			return true;
		}

		public WorldWidgetHandle ShowWorldWidget<TContext>(WorldWidgetType<TContext> type, TContext context) where TContext : WorldWidgetContext {
			return this.client.ShowWorldWidget(type, context);
		}

		public void UpdateWorldWidget<TContext>(WorldWidgetHandle handle, TContext context) where TContext : WorldWidgetContext {
			this.client.UpdateWorldWidget(handle, context);
		}

		public void DestroyWorldWidget(WorldWidgetHandle handle) {
			this.client.DestroyWorldWidget(handle);
		}

		public IScreenHandle OpenSignEditScreen(SignTileEntity signEntity) {
			return this.client.OpenScreen(new SignEditScreen(signEntity));
		}

		public void DropMainHandItem(bool ctrl) {
			ItemStack mainHandStack = this.GetMainHandStack();
			if (mainHandStack.IsEmpty()) return;

			int throwAmount = ctrl ? mainHandStack.count : 1;
			ItemStack thrownStack = mainHandStack.CopyWithCount(throwAmount);
			this.SetMainHandStack(mainHandStack.DecrementBy(throwAmount));

			ItemEntity itemEntity = this.DropStack(this.level, thrownStack);
			itemEntity.SetPickupDelay(DROP_PICKUP_DELAY);
		}

		public ItemStack Take(ItemStack itemStack) {
			ItemStack original = itemStack;
			if (this.inventory.TryAddStack(ref itemStack) || original.count != itemStack.count) {
				this.activeInventoryScreenHandler?.OnContentChanged(this.inventory);
			}
			return itemStack;
		}

		public bool CanPlaceBlockAt(BlockPos blockPos) {
			Vec2d worldPos = blockPos.ToVec2d();
			return this.IsInBlockReach(worldPos)
				   && this.level?.GetBlock(blockPos) == Blocks.AIR;
		}

		public bool CanBreakBlockAt(BlockPos blockPos) {
			Vec2d worldPos = blockPos.ToVec2d();
			return this.IsInBlockReach(worldPos)
				   && this.level?.GetBlock(blockPos) != Blocks.AIR;
		}

		public bool IsInBlockReach(Vec2d worldPos) {
			return this.boundingBox.SqrDistanceTo(worldPos) <= MAX_BLOCK_REACH * MAX_BLOCK_REACH;
		}

		public PlayerInventory GetInventory() => this.inventory;

		// temporary loot table implementation
		public float GetLuck() => 0f;

		public ItemStack GetMainHandStack() {
			ItemStack transitStack = this.GetTransitStack() ?? ItemStack.EMPTY;
			return transitStack.IsEmpty() ? this.inventory.GetMainStack() : transitStack;
		}

		public ItemStack? GetTransitStack() => this.activeInventoryScreenHandler?.GetTransitStack();

		public void SetMainHandStack(ItemStack stack) {
			this.CancelItemUse();
			this.SetMainHandStackInternal(stack);
		}

		private void SetMainHandStackInternal(ItemStack stack) {
			if (ItemStack.AreEqual(stack, this.GetMainHandStack())) return;
			if (this.activeInventoryScreenHandler != null && !this.activeInventoryScreenHandler.GetTransitStack().IsEmpty()) {
				this.activeInventoryScreenHandler.SetTransitStack(stack);
			} else {
				this.inventory.SetMainStack(stack);
			}
		}

		public void SetMainSlot(int slot) => this.inventory.SetMainSlot(slot);
		public int GetMainSlot() => this.inventory.GetMainSlot();

		public void SetScreenPointerPos(Vector2 pos) => this.screenPointerPos = pos;

		public Vector2 GetScreenPointerPos() => this.screenPointerPos;
		public Vec2d GetWorldPointerPos() {
			// still "depends" on Unity internals, just hidden behind the client layer
			// keep in mind PlayerEntity is at core layer, independent of UnityEngine
			return this.client.ScreenToWorldPoint(this.screenPointerPos);
		}

		public bool IsUsingItem() => this.activeItemUse != null;

		public void SetMovementX(float movementX) {
			this.movementX = movementX;
		}
	}
}
