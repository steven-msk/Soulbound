using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Common;
using SoulboundEngine.Common.Math;
using SoulboundEngine.Core.States;

namespace SoulboundEngine.Client.World.BlockSystem {
	[PROTOTYPICAL]
	public sealed class MovingTickingBlock : Block, ITickingBlock, INeighborUpdateHandler {
		public static readonly Property<bool> moving = BoolProperty.Of("canMove");
		private readonly BlockState movingState;
		private readonly BlockState staticState;

		public MovingTickingBlock(Settings settings) 
			: base(settings) {
			this.SetDefaultState(this.DefaultState.With(moving, true));
			this.movingState = this.DefaultState;
			this.staticState = this.DefaultState.With(moving, false);
		}

		protected override void AppendProperties(StateManager<Block, BlockState>.Builder builder) {
			builder.Add(moving);
		}

		void ITickingBlock.Tick(Level level, BlockPos blockPos, BlockState blockState) {
			if (!blockState.Get(moving)) return;

			BlockPos nextPos = this.GetNextPos(blockPos);
			if (level.GetBlockState(nextPos) == Blocks.air.DefaultState) {
				level.SetBlockState(blockPos, Blocks.air.DefaultState);
				level.SetBlockState(nextPos, this.movingState);
			} else {
				level.SetBlockState(blockPos, this.staticState);
			}
		}

		void INeighborUpdateHandler.OnNeighborChanged(Level level, BlockPos selfPos, BlockPos neighborPos) {
			BlockPos nextPos = this.GetNextPos(selfPos);
			if (nextPos != neighborPos) return;

			BlockState neighborState = level.GetBlockState(nextPos);
			BlockState selfState = level.GetBlockState(selfPos);
			if (neighborState == Blocks.air.DefaultState && !selfState.Get(moving)) {
				level.SetBlockState(selfPos, this.movingState);
			}
		}

		private BlockPos GetNextPos(BlockPos selfPos) => selfPos.GetAdjacent(Direction.Left); 

	}
}
