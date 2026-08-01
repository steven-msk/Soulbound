using SoulboundEngine.Client.Item;
using SoulboundEngine.Client.World.Block.State;
using SoulboundEngine.Client.World.Entity;
using SoulboundEngine.Core.States;
using System;
using System.Collections.Generic;

#nullable enable

namespace SoulboundEngine.Client.World.Block {
	using Item = Item.Item;

	public class Block : IItemConvertible {
		private static readonly List<BlockState> statesByID = new();
		private readonly Settings settings;
		private BlockState defaultState;
		protected StateManager<Block, BlockState> stateManager;

		public Block(Settings settings) {
			this.settings = settings;

			StateManager<Block, BlockState>.Builder builder = new(this);
			this.AppendProperties(builder);


			this.stateManager = builder.Build((owner, propertyMap) => {
				BlockState state = new(owner, propertyMap);
				statesByID.Add(state);
				return state;
			});

			this.defaultState = this.stateManager.defaultState;
		}

		protected virtual void AppendProperties(StateManager<Block, BlockState>.Builder builder) {
		}

		protected void SetDefaultState(BlockState blockState) {
			this.defaultState = blockState;
		}

		public BlockState DefaultState => this.defaultState;

		public StateManager<Block, BlockState> StateManager => this.stateManager;

		public Item AsItem() {
			return Item.blockItems.TryGetValue(this, out Item item) ? item : Items.AIR;
		}

		public static Block GetBlockFrom(Item? item) {
			if (item == null) return Blocks.AIR;
			return item is BlockItem blockItem
				? blockItem.GetBlock()
				: Blocks.AIR;
		}

		public static void DropStacks(BlockState blockState, Level.Level level, BlockPos blockPos, World.Entity.Entity? owner) {
			List<ItemStack> droppedStacks = GetDroppedStacks(blockState);

			foreach (var stack in droppedStacks) {
				ItemEntity itemEntity = new(owner, stack, level);
				itemEntity.SetPosition(blockPos.GetCenter());
				level.AddEntity(itemEntity);
			}
		}

		public static List<ItemStack> GetDroppedStacks(BlockState blockState) {
			return blockState.block.settings.droppedStacks(blockState);
		}

		private static Func<BlockState, List<ItemStack>> DropSingle() => blockState => {
			return new List<ItemStack>() { blockState.block.AsItem().GetDefaultStack(1) };
		};

		private static Func<BlockState, List<ItemStack>> DropAir() => _ => {
			return new List<ItemStack>();
		};

		public string name => this.settings.name;
		public int minBreakLevel => this.settings.minBreakLevel;

		public static int GetRawID(BlockState state) {
			return statesByID.IndexOf(state);
		}

		public static BlockState GetState(int id) {
			return statesByID[id];
		}

		public abstract class AbstractBlockState : State<Block, BlockState> {
			protected AbstractBlockState(Block owner, Entries entries) : base(owner, entries) {
			}

			protected abstract BlockState AsBlockState();

			public List<ItemStack> GetDroppedStacks() {
				return Block.GetDroppedStacks(this.AsBlockState());
			}
		}

		public sealed class Settings {
			public string name { get; private set; }
			public int minBreakLevel { get; private set; } = 0;
			public Func<BlockState, List<ItemStack>> droppedStacks { get; private set; } = DropSingle();

			private Settings(string name) {
				this.name = name;
			}

			public static Settings Of(string name) {
				return new Settings(name);
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
				this.droppedStacks = DropAir();
				return this;
			}
		}
	}
}
