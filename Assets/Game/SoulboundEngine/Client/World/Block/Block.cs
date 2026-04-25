using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.World.BlockSystem.Render;
using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Client.World.BlockSystem.TileEntities;
using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Core.States;
using System.Collections.Generic;

#nullable enable

namespace SoulboundEngine.Client.World.BlockSystem {
	public abstract class Block {
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

		public abstract BlockRenderData GetRenderData(BlockState blockState);

		public virtual bool HasTileEntity(Level level, BlockPos blockPos, BlockState blockState) => false;
		public virtual TileEntity? GetTileEntity(Level level, BlockPos blockPos) {
			return null;
		}

		public virtual BlockState Place(ItemStack itemStack, BlockPos blockPos) {
			return this.defaultState;
		}
		public virtual void OnNeighborStateChanged(BlockPos selfPos, BlockPos neighborPos, BlockState? oldState, BlockState? newState) {
		}
		public virtual IEnumerable<ItemStack> GetDrops(BlockState blockState, BreakSource source) {
			yield break;
		}

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
		}

		public sealed class Settings {
			public string name { get; private set; }
			public int minBreakLevel { get; private set; } = 0;

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
		}
	}
}
