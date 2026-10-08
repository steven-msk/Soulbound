namespace SoulboundEngine.World.Block {
	using SoulboundEngine.Common.Math;
	using SoulboundEngine.Item;
	using SoulboundEngine.Loot.Context;
	using SoulboundEngine.Registry;
	using SoulboundEngine.Serialization;
	using SoulboundEngine.State;
	using SoulboundEngine.World.Block.State;
	using SoulboundEngine.World.Entity;
	using SoulboundEngine.World.Level;
	using SoulboundEngine.World.Player;
	using System;
	using System.Collections.Generic;

#nullable enable

	public class Block : AbstractBlock {
		public static readonly Codec<RegistryEntry<Block>> ENTRY_CODEC = RegistryEntry<Block>.GetCodec(Registries.BLOCK);
		public static readonly Codec<Block> CODEC = ENTRY_CODEC.Xmap(e => e.GetValue(), Registries.BLOCK.GetEntry);
		private static readonly List<BlockState> STATES_BY_ID = new();
		private readonly RegistryKey<Block> registryKey;
		private BlockState defaultState;
		protected StateManager<Block, BlockState> stateManager;

		public Block(AbstractBlock.Settings settings) : base(settings) {
			this.registryKey = settings.registryKey ?? throw new NotSupportedException("Block is not added to a registry");

			StateManager<Block, BlockState>.Builder builder = new(this);
			this.AppendProperties(builder);

			this.stateManager = builder.Build((owner, propertyMap) => {
				BlockState state = new(owner, propertyMap);
				STATES_BY_ID.Add(state);
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

		public static Block GetBlockFrom(Item? item) {
			return item == null || item is not BlockItem blockItem ? Blocks.AIR : blockItem.GetBlock();
		}

		public override ToolPower GetRequiredToolPower() => this.settings.requiredToolPower;

		public override float GetHardness(BlockState blockState) => this.settings.hardness;

		public static void DropStacks(BlockState blockState, Level level, BlockPos blockPos, World.Entity.Entity? owner) {
			LootWorldContext.Builder context = new(level);
			if (owner is PlayerEntity player) context.Luck(player.GetLuck());
			IReadOnlyList<ItemStack> droppedStacks = blockState.GetDrops(context);

			foreach (ItemStack stack in droppedStacks) {
				if (stack.IsEmpty()) continue;
				Vec2d pos = blockPos.GetBottomCenter();
				ItemEntity itemEntity = new(level, pos.x, pos.y, stack);
				if (owner != null) itemEntity.SetOwner(owner);
				level.AddNewEntity(itemEntity);
			}
		}

		public string GetTranslationKey() => this.settings.GetTranslationKey();

		public static int GetRawID(BlockState state) {
			return STATES_BY_ID.IndexOf(state);
		}

		public static BlockState GetState(int id) {
			return STATES_BY_ID[id];
		}

		public override string ToString() {
			return this.registryKey.value.ToString();
		}
	}
}
