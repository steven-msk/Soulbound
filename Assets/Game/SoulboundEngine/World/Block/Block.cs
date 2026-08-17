

namespace SoulboundEngine.World.Block {
	using SoulboundEngine.Common.Math;
	using SoulboundEngine.Item;
	using SoulboundEngine.Registry;
	using SoulboundEngine.States;
	using SoulboundEngine.World.Block.State;
	using SoulboundEngine.World.Entity;
	using SoulboundEngine.World.Level;
	using SoulboundEngine.World.Player;
	using System;
	using System.Collections.Generic;

#nullable enable

	public class Block : AbstractBlock {
		private static readonly List<BlockState> statesByID = new();
		private readonly RegistryKey<Block> registryKey;
		private readonly AbstractBlock.Settings settings;
		private BlockState defaultState;
		protected StateManager<Block, BlockState> stateManager;

		public Block(AbstractBlock.Settings settings) {
			this.settings = settings;
			this.registryKey = settings.registryKey ?? throw new NotSupportedException("Block is not added to a registry");

			StateManager<Block, BlockState>.Builder builder = new(this);
			this.AppendProperties(builder);


			this.stateManager = builder.Build((owner, propertyMap) => {
				BlockState state = new(owner, propertyMap);
				statesByID.Add(state);
				return state;
			});

			this.defaultState = this.stateManager.defaultState;
		}

		public static Block Create(AbstractBlock.Settings settings) => new(settings);

		protected virtual void AppendProperties(StateManager<Block, BlockState>.Builder builder) {
		}

		protected void SetDefaultState(BlockState blockState) {
			this.defaultState = blockState;
		}

		public BlockState DefaultState => this.defaultState;

		public StateManager<Block, BlockState> StateManager => this.stateManager;

		public sealed override Item AsItem() {
			return Item.blockItems.TryGetValue(this, out Item item) ? item : Items.AIR;
		}

		protected sealed override Block AsBlock() => this;

		public override RegistryKey<Block> GetKey() => this.registryKey;

		protected override BlockShape GetShape(BlockState state, BlockPos blockPos, Level level) {
			return BlockShape.FULL;
		}

		public virtual BlockState OnBreak(Level level, BlockPos blockPos, BlockState blockState, PlayerEntity player) {
			return Blocks.AIR.DefaultState;
		}

		public static Block GetBlockFrom(Item? item) {
			if (item == null) return Blocks.AIR;
			return item is BlockItem blockItem
				? blockItem.GetBlock()
				: Blocks.AIR;
		}

		public static void DropStacks(BlockState blockState, Level level, BlockPos blockPos, World.Entity.Entity? owner) {
			List<ItemStack> droppedStacks = GetDroppedStacks(blockState);

			foreach (var stack in droppedStacks) {
				Vec2d pos = blockPos.GetBottomCenter();
				ItemEntity itemEntity = new(level, pos.x, pos.y, stack);
				if (owner != null) itemEntity.SetOwner(owner);
				level.AddEntity(itemEntity);
			}
		}

		public static List<ItemStack> GetDroppedStacks(BlockState blockState) {
			return blockState.block.settings.droppedStacks(blockState);
		}

		internal protected static Func<BlockState, List<ItemStack>> DropSingle() => blockState => {
			return new List<ItemStack>() { blockState.block.AsItem().GetDefaultStack(1) };
		};

		internal protected static Func<BlockState, List<ItemStack>> DropAir() => _ => {
			return new List<ItemStack>();
		};

		public int MinBreakLevel => this.settings.minBreakLevel;

		public string GetTranslationKey() => this.settings.GetTranslationKey();

		public static int GetRawID(BlockState state) {
			return statesByID.IndexOf(state);
		}

		public static BlockState GetState(int id) {
			return statesByID[id];
		}

		public override string ToString() {
			return this.registryKey.value.ToString();
		}
	}
}
