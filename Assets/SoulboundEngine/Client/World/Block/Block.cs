using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.World.BlockSystem.Render;
using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Client.World.BlockSystem.TileEntities;
using SoulboundEngine.Client.World.LevelDomain;
using System.Collections.Generic;

#nullable enable

namespace SoulboundEngine.Client.World.BlockSystem {
	public abstract partial class Block {
		public abstract string name { get; init; }
		public abstract int minBreakLevel { get; init; }
		public BlockState defaultState { get; private set; } = null!;

		protected Block(IBlockStateRegisterer? stateRegisterer = null) {
			RegisterStates(stateRegisterer);
		}

		protected Block(string name, int minBreakLevel, IBlockStateRegisterer? stateRegisterer = null) {
			this.name = name;
			this.minBreakLevel = minBreakLevel;
			RegisterStates(stateRegisterer);
		}

		private void RegisterStates(IBlockStateRegisterer? stateRegisterer) {
			BlockPropertyEntries properties = new();
			stateRegisterer ??= new GlobalBlockStateRegisterer();
			stateRegisterer.SetBlock(this);

			CreateStates(stateRegisterer, properties);
			defaultState = GetDefaultState(stateRegisterer, properties);
			stateRegisterer.Add(defaultState);

			stateRegisterer.FinishRegistry();
		}

		public abstract BlockRenderData GetRenderData(BlockState blockState);

		protected virtual BlockState GetDefaultState(IBlockStateRegisterer registerer, BlockPropertyEntries properties) {
			return registerer.AddWithProperties(properties);
		}

		protected virtual void CreateStates(IBlockStateRegisterer registerer, BlockPropertyEntries properties) {
		}

		public virtual bool HasTileEntity(Level level, BlockPos blockPos, BlockState blockState) => false;
		public virtual TileEntity? GetTileEntity(Level level, BlockPos blockPos) {
			return null;
		}

		public virtual BlockState Place(ItemStack itemStack, BlockPos blockPos) {
			return defaultState;
		}
		public virtual void OnNeighborStateChanged(BlockPos selfPos, BlockPos neighborPos, BlockState? oldState, BlockState? newState) {
		}
		public virtual IEnumerable<ItemStack> GetDrops(BlockState blockState, BreakSource source) {
			yield break;
		}
	}
}
