using SoulboundEngine.World.Level;
using SoulboundEngine.Core.Event;
using SoulboundEngine.World.Block;
using SoulboundEngine.World.Block.State;

namespace SoulboundEngine.Client {
	public struct BlockPlacedEvent : IGameEvent {
		public BlockState blockState;
		public BlockPos blockPos;
		public Level level;

		public BlockPlacedEvent(BlockState blockState, BlockPos blockPos, Level level) {
			this.blockState = blockState;
			this.blockPos = blockPos;
			this.level = level;
		}
	}
}
