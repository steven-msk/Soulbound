using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.World.BlockSystem.States;
using SoulboundBackend.Client.World.LevelDomain;
using SoulboundBackend.Common;
using SoulboundBackend.Common.Math;
using SoulboundBackend.Core.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.BlockSystem {
	[PROTOTYPICAL]
	public sealed class MovingTickingBlock : Block, ITickingBlock, INeighborUpdateHandler {
		private BlockState movingState;
		private BlockState staticState;
		public override string name { get; init; } = "Moving Ticking Block";
		public override int minBreakLevel { get; init; } = 0;

		public override AssetKey GetRenderTileKey(BlockState blockState) => new("WhiteSquareTile");

		public MovingTickingBlock() : base("movingTickingBlock") {
		}

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

		protected override void CreateStates(BlockStateRegisterer registerer, BlockPropertyEntries properties) {
			movingState = registerer.AddWithProperties(properties.With("canMove", true));
			staticState = registerer.AddWithProperties(properties.With("canMove", false));
		}

		protected override BlockState GetDefaultState(BlockStateRegisterer registerer, BlockPropertyEntries properties) {
			return movingState;
		}
	}
}
