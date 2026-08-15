using SoulboundEngine.Client.World;
using SoulboundEngine.Client.World.Level;
using SoulboundEngine.World.Block.State;

namespace SoulboundEngine.World.Block {
	public interface ITickingBlock {
		void Tick(Level level, BlockPos blockPos, BlockState blockState);
	}
}
