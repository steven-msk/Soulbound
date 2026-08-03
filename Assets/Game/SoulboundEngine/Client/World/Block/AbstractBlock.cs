using SoulboundEngine.Client.Interaction;
using SoulboundEngine.Client.Item;
using SoulboundEngine.Client.Player;
using SoulboundEngine.Client.World.Block.State;
using SoulboundEngine.Common.Math;
using SoulboundEngine.Core.Registry;
using SoulboundEngine.Core.States;
using System;
using System.Collections.Generic;

namespace SoulboundEngine.Client.World.Block {
	using Item = Item.Item;
	using Level = Level.Level;

	public abstract class AbstractBlock : IItemConvertible {
		public abstract Item AsItem();
		protected abstract Block AsBlock();

		public abstract RegistryKey<Block> GetKey();

		protected virtual bool CanPlaceAt(BlockState blockState, Level level, BlockPos blockPos) {
			if (level.GetBlock(blockPos) != Blocks.AIR) return false;

			foreach (var pos in blockPos.GetCardinalNeighbors()) {
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

		protected virtual bool IsInteractable(BlockState blockState, Level level, BlockPos blockPos) => true;

		public abstract class AbstractBlockState : State<Block, BlockState> {
			protected AbstractBlockState(Block owner, Entries entries) : base(owner, entries) {
			}

			protected abstract BlockState AsBlockState();

			public List<ItemStack> GetDroppedStacks() {
				return Block.GetDroppedStacks(this.AsBlockState());
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

			public Block GetBlock() => this.owner;
		}

		public sealed class Settings {
			public RegistryKey<Block> registryKey { get; private set; }
			public int minBreakLevel { get; private set; } = 0;
			public Func<BlockState, List<ItemStack>> droppedStacks { get; private set; } = Block.DropSingle();

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
				if (this.registryKey is null) {
					throw new InvalidOperationException("Cannot derive block name: RegistryKey was not set before Build() was called.");
				}
				return this.registryKey.value.ToTranslationKey("block");
			}
		}
	}
}
