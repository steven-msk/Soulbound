using SoulboundEngine.World.Block;
using SoulboundEngine.World.Block.State;
using SoulboundEngine.World.Level;

namespace SoulboundEngine.Event {
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
