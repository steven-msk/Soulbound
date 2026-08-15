using SoulboundEngine.World.Level;
using SoulboundEngine.Core.Event;
using SoulboundEngine.World.Block;

namespace SoulboundEngine.Client {
	public struct BlockBrokenEvent : IGameEvent {
		public BlockPos blockPos;
		public Level level;

		public BlockBrokenEvent(BlockPos blockPos, Level level) {
			this.blockPos = blockPos;
			this.level = level;
		}
	}
}
