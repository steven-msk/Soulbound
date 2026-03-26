using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.LevelDomain;
using SoulboundBackend.Core.Event;

namespace SoulboundBackend.Client {
	public struct BlockBrokenEvent : IGameEvent {
		public BlockPos blockPos;
		public Level level;

		public BlockBrokenEvent(BlockPos blockPos, Level level) {
			this.blockPos = blockPos;
			this.level = level;
		}
	}
}
