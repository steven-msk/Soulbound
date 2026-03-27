using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Core.Event;

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
