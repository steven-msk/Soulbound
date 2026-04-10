using SoulboundEngine.Client.World.BlockSystem.Render;
using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Common;
using SoulboundEngine.Common.Math;
using SoulboundEngine.Core.Assets;

namespace SoulboundEngine.Client.World.BlockSystem {
	[PROTOTYPICAL]
	public sealed class MovingTickingBlock : Block, ITickingBlock, INeighborUpdateHandler {
		private BlockState movingState;
		private BlockState staticState;
		public override string name { get; init; } = "Moving Ticking Block";
		public override int minBreakLevel { get; init; } = 0;

		void ITickingBlock.Tick(Level level, BlockPos blockPos, BlockState blockState) {
			if (!blockState.Get<bool>("canMove")) return;

			BlockPos nextPos = GetNextPos(blockPos);
			if (level.GetBlockState(nextPos) == Blocks.air.defaultState) {
				level.SetBlockState(blockPos, Blocks.air.defaultState);
				level.SetBlockState(nextPos, movingState);
			} else {
				level.SetBlockState(blockPos, staticState);
			}
		}

		void INeighborUpdateHandler.OnNeighborChanged(Level level, BlockPos selfPos, BlockPos neighborPos) {
			BlockPos nextPos = GetNextPos(selfPos);
			if (nextPos != neighborPos) return;

			BlockState neighborState = level.GetBlockState(nextPos);
			BlockState selfState = level.GetBlockState(selfPos);
			if (neighborState == Blocks.air.defaultState && !selfState.Get<bool>("canMove")) {
				level.SetBlockState(selfPos, movingState);
			}
		}

		private BlockPos GetNextPos(BlockPos selfPos) => selfPos.GetAdjacent(Direction.Left); 

		protected override void CreateStates(IBlockStateRegisterer registerer, BlockPropertyEntries properties) {
			movingState = registerer.AddWithProperties(properties.With("canMove", true));
			staticState = registerer.AddWithProperties(properties.With("canMove", false));
		}

		protected override BlockState GetDefaultState(IBlockStateRegisterer registerer, BlockPropertyEntries properties) {
			return movingState;
		}

		public override BlockRenderData GetRenderData(BlockState blockState) {
			return new BlockRenderData(new AssetKey("WhiteSquareTile"));
		}
	}
}
