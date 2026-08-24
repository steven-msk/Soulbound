namespace SoulboundEngine.World.Block {
	using SoulboundEngine.Common.Math;
	using SoulboundEngine.Interaction;
	using SoulboundEngine.Item;
	using SoulboundEngine.Registry;
	using SoulboundEngine.States;
	using SoulboundEngine.World.Block.State;
	using SoulboundEngine.World.Player;
	using System;
	using System.Collections.Generic;
	using Item = Item.Item;
	using Level = Level.Level;

	public abstract class AbstractBlock : IItemConvertible {
		public abstract Item AsItem();
		protected abstract Block AsBlock();

		public abstract RegistryKey<Block> GetKey();

		protected abstract BlockShape GetShape(BlockState state, BlockPos blockPos, Level level);

		protected virtual void OnStateReplaced(BlockState state, BlockPos pos, Level level) {
		}

		protected virtual bool CanPlaceAt(BlockState blockState, Level level, BlockPos blockPos) {
			if (level.GetBlock(blockPos) != Blocks.AIR) return false;

			foreach (BlockPos pos in blockPos.GetCardinalNeighbors()) {
				if (level.GetBlock(pos) != Blocks.AIR) return true;
			}
			return false;
		}

		/// <summary> 
		/// Called when the player interacts with this block (left click).
		/// This is called after <see cref="OnPrimaryUseWithItem(BlockState, ItemStack, Level, PlayerEntity, BlockPos)"/> is called with a non-empty item.
		/// </summary>
		protected virtual IActionResult OnPrimaryUse(BlockState state, Level level, PlayerEntity player, BlockPos pos) => IActionResult.PASS;

		/// <summary> 
		/// Called when the player interacts with this block (right click).
		/// This is called after <see cref="OnSecondaryUseWithItem(BlockState, ItemStack, Level, PlayerEntity, BlockPos)"/> is called with a non-empty item.
		/// </summary>
		protected virtual IActionResult OnSecondaryUse(BlockState state, Level level, PlayerEntity player, BlockPos pos) => IActionResult.PASS;

		/// <summary> Called when the player interacts with this block (left click) with an item </summary>
		protected virtual IActionResult OnPrimaryUseWithItem(BlockState state, ItemStack stack, Level level, PlayerEntity player, BlockPos pos) => IActionResult.PASS;

		/// <summary> Called when the player interacts with this block (right click) with an item </summary>
		protected virtual IActionResult OnSecondaryUseWithItem(BlockState state, ItemStack stack, Level level, PlayerEntity player, BlockPos pos) => IActionResult.PASS;

		/// <summary> Called when the player's pointer enters this block. </summary>
		protected virtual void OnHoverEnter(BlockState state, ItemStack stack, Level level, PlayerEntity player, BlockPos pos) {
		}

		/// <summary> Called every tick while the player's pointer is over this block. </summary>
		protected virtual void OnHoverTick(BlockState state, ItemStack stack, Level level, PlayerEntity player, BlockPos pos) {
		}

		/// <summary> Called when the player's pointer leaves this block. </summary>
		protected virtual void OnHoverLeave(BlockState state, ItemStack stack, Level level, PlayerEntity player, BlockPos pos) {
		}

		protected virtual bool IsInteractable(BlockState blockState, Level level, BlockPos blockPos) => true;

		protected virtual bool ShouldChangedStateKeepTileEntity(BlockState oldState) {
			return false;
		}

		protected virtual void OnPlace(Level level, BlockPos blockPos, BlockState oldState) {
		}

		public abstract class AbstractBlockState : State<Block, BlockState> {
			protected AbstractBlockState(Block owner, Entries entries) : base(owner, entries) {
			}

			protected abstract BlockState AsBlockState();

			public BlockShape GetCollisionShape(BlockState state, Level level, BlockPos blockPos) {
				return this.owner.GetShape(state, blockPos, level);
			}

			public List<ItemStack> GetDroppedStacks() {
				return Block.GetDroppedStacks(this.AsBlockState());
			}

			public void OnStateReplaced(BlockPos pos, Level level) {
				this.owner.OnStateReplaced(this.AsBlockState(), pos, level);
			}

			public bool CanPlaceAt(Level level, BlockPos blockPos) {
				return this.owner.CanPlaceAt(this.AsBlockState(), level, blockPos);
			}

			public bool IsInteractable(Level level, BlockPos blockPos) {
				return this.owner.IsInteractable(this.AsBlockState(), level, blockPos);
			}

			public static IActionResult OnPrimaryUse(BlockState state, Level level, PlayerEntity player, BlockPos blockPos) {
				return state.owner.OnPrimaryUse(state, level, player, blockPos);
			}

			public static IActionResult OnSecondaryUse(BlockState state, Level level, PlayerEntity player, BlockPos pos) {
				return state.owner.OnSecondaryUse(state, level, player, pos);
			}

			public static IActionResult OnPrimaryUseWithItem(BlockState state, ItemStack stack, Level level, PlayerEntity player, BlockPos pos) {
				return state.owner.OnPrimaryUseWithItem(state, stack, level, player, pos);
			}

			public static IActionResult OnSecondaryUseWithItem(BlockState state, ItemStack stack, Level level, PlayerEntity player, BlockPos pos) {
				return state.owner.OnSecondaryUseWithItem(state, stack, level, player, pos);
			}

			public void OnHoverEnter(ItemStack stack, Level level, PlayerEntity player, BlockPos pos) {
				this.owner.OnHoverEnter(this.AsBlockState(), stack, level, player, pos);
			}

			public void OnHoverTick(ItemStack stack, Level level, PlayerEntity player, BlockPos pos) {
				this.owner.OnHoverTick(this.AsBlockState(), stack, level, player, pos);
			}

			public void OnHoverLeave(ItemStack stack, Level level, PlayerEntity player, BlockPos pos) {
				this.owner.OnHoverLeave(this.AsBlockState(), stack, level, player, pos);
			}

			public Block GetBlock() => this.owner;

			public bool HasTileEntity() => this.owner is ITileEntityProvider;

			public bool ShouldChangedStateKeepTileEntity(BlockState oldState) {
				return this.owner.ShouldChangedStateKeepTileEntity(oldState);
			}

			public void OnPlace(Level level, BlockPos blockPos, BlockState oldState) {
				this.owner.OnPlace(level, blockPos, oldState);
			}
		}

		public sealed class Settings {
			public RegistryKey<Block> registryKey { get; private set; }
			public int minBreakLevel { get; private set; } = 0;
			public Func<BlockState, List<ItemStack>> droppedStacks { get; private set; } = Block.DropAir();

			public Settings RegistryKey(RegistryKey<Block> registryKey) {
				this.registryKey = registryKey;
				return this;
			}

			public Settings MinBreakLevel(int minBreakLevel) {
				this.minBreakLevel = minBreakLevel;
				return this;
			}

			public Settings Drops(Func<BlockState, List<ItemStack>> droppedStacks) {
				this.droppedStacks = droppedStacks;
				return this;
			}

			public Settings DropsAir() {
				this.droppedStacks = Block.DropAir();
				return this;
			}

			public string GetTranslationKey() {
				return this.registryKey is null
					? throw new InvalidOperationException("Cannot derive block name: RegistryKey was not set before Build() was called.")
					: this.registryKey.value.ToTranslationKey("block");
			}
		}
	}
}
