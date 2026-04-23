using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Core.Event;

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
