namespace SoulboundEngine.World.Player {
	using Newtonsoft.Json.Linq;
	using SoulboundEngine.Common.Math;
	using SoulboundEngine.Interaction;
	using SoulboundEngine.Inventory;
	using SoulboundEngine.Item;
	using SoulboundEngine.Item.Container;
	using SoulboundEngine.Serialization;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Block.Entity;
	using SoulboundEngine.World.Block.State;
	using SoulboundEngine.World.Entity;
	using SoulboundEngine.World.Entity.Attribute;
	using SoulboundEngine.World.Level;
	using SoulboundEngine.World.Physics;
	using System;
	using System.Collections.Generic;

#nullable enable

	public abstract class PlayerEntity : Entity {
		private const double MAX_BLOCK_REACH = 5d;
		private const double PICKUP_RANGE_X = 0.75d;
		private const double PICKUP_RANGE_Y = 0.5d;
		private const int DROP_PICKUP_DELAY_TICKS = 75;
		private readonly PlayerInventory inventory;
		private readonly BlockBreakManager blockBreakManager;
		private Vec2d screenPointerPos;
		private ActiveUseContext? activeItemUse;
		private BlockPos previousPointerBlockPos;
		private bool isHoldingLeft;
		private bool isHoldingRight;

		public PlayerEntity(Level level)
			: base(EntityType.PLAYER, level) {
			this.inventory = new PlayerInventory(this);
			this.blockBreakManager = new BlockBreakManager();
		}

		public new static AttributeSupplier.Builder CreateDefaultAttributes() {
			return Entity.CreateDefaultAttributes()
				.Add(Attributes.SPEED, 0.1f)
				.Add(Attributes.GRAVITY, 0.019622225d)
				.Add(Attributes.JUMP_POWER)
				.Add(Attributes.LUCK);
		}

		public bool isJumping { get; private set; }
		public float movementX { get; private set; }

		public abstract void OpenInventoryScreen(IInventoryScreenHandlerFactory handlerFactory);

		public abstract bool IsInventoryOpen();

		public abstract void CloseInventoryScreen();

		public void ToggleInventory() {
			if (!this.IsInventoryOpen()) {
				this.OpenInventoryScreen(new DelegatedInventoryScreenHandlerFactory(
					(inventory, _) => {
						InventoryScreenHandlerContext context = InventoryScreenHandlerContext.Of(BlockPos.From(this.GetPosition()), this.level);
						return new PlayerInventoryScreenHandler(InventoryScreenHandlerType.PLAYER_INVENTORY, inventory, context);
					}
				));
			} else {
				this.CloseInventoryScreen();
			}
		}

		public abstract InventoryScreenHandler? GetInventoryScreenHandler();

		public override void Tick() {
			this.inventory.Tick();
			Vec2d movementInput = new(this.movementX, 0.0d);
			this.Travel(movementInput);

			base.Tick();
			this.DoBlockHover();
			this.CheckItemUse();
			if (this.isHoldingLeft) this.OnLeftHoldTick();
			if (this.isHoldingRight) this.OnRightHoldTick();

			InventoryScreenHandler? inventoryScreenHandler = this.GetInventoryScreenHandler();
			if (inventoryScreenHandler != null && !inventoryScreenHandler.CanUse(this)) {
				this.CloseInventoryScreen();
			}

			List<Entity> collidedEntities = this.level.GetEntities(this, this.GetPickupArea(), ALL);
			foreach (Entity entity in collidedEntities) {
				this.Touch(entity);
			}

			if (this.isJumping && this.IsOnGround()) {
				this.JumpFromGround();
			}
		}

		private AABB GetPickupArea() {
			return this.boundingBox.Stretch(PICKUP_RANGE_X, PICKUP_RANGE_Y);
		}

		private void Touch(Entity entity) {
			entity.PlayerTouch(this);
		}

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
			BlockPos blockPos = BlockPos.From(this.GetWorldPointerPos());
			if (this.activeItemUse?.type == type) {
				this.HandleUseTick();
			} else if (this.activeItemUse == null && this.GetMainHandStack().ShouldContinueInteraction(type, this.level, this, blockPos)) {
				switch (type) {
					case InteractionType.Primary:
						this.PrimaryInteract();
						break;
					case InteractionType.Secondary:
						this.SecondaryInteract();
						break;
					default:
						throw new ArgumentException();
				}
			}
		}

		private void PrimaryInteract() {
			if (!this.Interact(InteractionType.Primary,
					ItemStack.OnPrimaryUseOnEntity, ItemStack.OnPrimaryUseOnBlock, ItemStack.OnPrimaryUse,
					AbstractBlock.AbstractBlockState.OnPrimaryUse, AbstractBlock.AbstractBlockState.OnPrimaryUseWithItem
				)) {
				BlockPos blockPos = BlockPos.From(this.GetWorldPointerPos());
				ItemStack stack = this.GetMainHandStack();
				if (this.TryBreakBlock(blockPos, stack)) this.DoBlockBreakTick(blockPos, stack);
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
			BlockState blockState = this.level.GetBlockState(blockPos);
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
			return result is not IActionResult.PassToBlockAction && HandleActionResult(result, player);
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
			if (result.HasAction()) {
				player.SetMainHandStack(result.ReplaceStack(player.GetMainHandStack().Copy()));
			}
			return result.HasAction();
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
			return blockState != Blocks.AIR.DefaultState && this.IsInBlockReach(blockPos.GetCenter());
		}

		private bool TryBreakBlock(BlockPos blockPos, ItemStack stack) {
			if (!Level.IsInBounds(blockPos)) return false;
			if (!this.CanBreakBlockAt(blockPos)) return false;

			BlockState blockState = this.level.GetBlockState(blockPos) ?? Blocks.AIR.DefaultState;
			this.blockBreakManager.Reset(blockState, blockPos);
			return blockState.block != Blocks.AIR && stack.CanMine(blockState);
		}

		private void DoBlockBreakTick(BlockPos blockPos, ItemStack stack) {
			BlockState blockState = this.level.GetBlockState(blockPos);
			float speed = stack.GetMiningSpeed(blockState);

			if (this.blockBreakManager.Tick(speed)) {
				this.level.SetBlockState(blockPos, blockState.GetBreakState(this.level, blockPos, blockState));
				Block.DropStacks(blockState, this.level, blockPos, null);
				stack.DamageTool(this, EquipmentSlot.MAIN_HAND);
				this.SetMainHandStack(stack);
			}
		}

		public void JumpFromGround() {
			double jumpPower = this.GetJumpPower();
			if (jumpPower > 1.0E-5) {
				Vec2d movement = this.GetDeltaMovement();
				this.SetDeltaMovement(movement.x, Math.Max(jumpPower, movement.y));
			}
			this.SetOnGround(false);
		}

		private double GetJumpPower() => this.GetAttributeValue(Attributes.JUMP_POWER);

		public void SetJumping(bool jumping) {
			this.isJumping = jumping;
		}

		public abstract bool OpenSignEditScreen(SignTileEntity signEntity);

		public void DropMainHandItem(bool ctrl) {
			ItemStack mainHandStack = this.GetMainHandStack();
			if (mainHandStack.IsEmpty()) return;

			int throwAmount = ctrl ? mainHandStack.count : 1;
			ItemStack thrownStack = mainHandStack.CopyWithCount(throwAmount);
			this.SetMainHandStack(mainHandStack.DecrementBy(throwAmount));

			ItemEntity itemEntity = this.DropStack(thrownStack);
			itemEntity.SetPickupDelay(DROP_PICKUP_DELAY_TICKS);
		}

		public ItemStack Take(ItemStack itemStack) {
			ItemStack original = itemStack;
			if (this.inventory.TryAddStack(ref itemStack) || original.count != itemStack.count) {
				this.GetInventoryScreenHandler()?.OnContentChanged(this.inventory);
			}
			return itemStack;
		}

		public bool CanPlaceBlockAt(BlockPos blockPos) {
			Vec2d worldPos = blockPos.GetCenter();
			return this.IsInBlockReach(worldPos)
				&& !this.boundingBox.Overlaps(blockPos)
				&& this.level?.GetBlock(blockPos) == Blocks.AIR;
		}

		public bool CanBreakBlockAt(BlockPos blockPos) {
			Vec2d worldPos = blockPos.GetCenter();
			return this.IsInBlockReach(worldPos)
				   && this.level?.GetBlock(blockPos) != Blocks.AIR;
		}

		public bool IsInBlockReach(Vec2d worldPos) {
			return this.boundingBox.SqrDistanceTo(worldPos) <= MAX_BLOCK_REACH * MAX_BLOCK_REACH;
		}

		public PlayerInventory GetInventory() => this.inventory;

		public float GetLuck() => (float)this.GetAttributeValue(Attributes.LUCK);

		public ItemStack GetMainHandStack() => this.GetStack(EquipmentSlot.MAIN_HAND);

		public ItemStack? GetTransitStack() => this.GetInventoryScreenHandler()?.GetTransitStack();

		public void SetMainHandStack(ItemStack stack) {
			this.CancelItemUse();
			this.SetMainHandStackInternal(stack);
		}

		private void SetMainHandStackInternal(ItemStack stack) {
			if (ItemStack.AreEqual(stack, this.GetMainHandStack())) return;
			this.SetStack(EquipmentSlot.MAIN_HAND, stack);
		}

		public void SetMainSlot(int slot) => this.inventory.SetMainSlot(slot);
		public int GetMainSlot() => this.inventory.GetMainSlot();

		protected override EntityEquipment CreateEquipment() {
			return new PlayerEquipment(this);
		}

		public void SetScreenPointerPos(Vec2d pos) => this.screenPointerPos = pos;
		public Vec2d GetScreenPointerPos() => this.screenPointerPos;
		public abstract Vec2d GetWorldPointerPos();

		public bool IsUsingItem() => this.activeItemUse != null;

		public void SetMovementX(float movementX) {
			this.movementX = movementX;
		}

		protected override void SaveAdditional(JToken json) {
			base.SaveAdditional(json);
			json["inventory"] = this.inventory.Save();
			json["mainSlot"] = Codecs.INT.Encode(this.inventory.GetMainSlot());
		}

		protected override void LoadAdditional(JObject json) {
			base.LoadAdditional(json);
			this.inventory.Load(json["inventory"] ?? JValue.CreateNull());
			Codecs.INT.Decode(json["mainSlot"] ?? JValue.CreateNull())
				.ResultOrPartial().IfPresent(this.inventory.SetMainSlot);
		}

	}
}
