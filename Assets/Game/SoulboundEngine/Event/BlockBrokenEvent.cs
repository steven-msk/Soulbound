using SoulboundEngine.World.Block;
using SoulboundEngine.World.Level;

namespace SoulboundEngine.Event {
	public struct BlockBrokenEvent : IGameEvent {
		public BlockPos blockPos;
		public Level level;

		public BlockBrokenEvent(BlockPos blockPos, Level level) {
			this.blockPos = blockPos;
			this.level = level;
		}
	}
}
