using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.BlockSystem.States;
using SoulboundBackend.Client.World.LevelDomain;
using SoulboundBackend.Core.Event;

namespace SoulboundBackend.Client {
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
