using SoulboundEngine.World.Block.State;

namespace SoulboundEngine.World.Block {
	using Level = Level.Level;

	public interface ITickingBlock {
		void Tick(Level level, BlockPos blockPos, BlockState blockState);
	}
}
