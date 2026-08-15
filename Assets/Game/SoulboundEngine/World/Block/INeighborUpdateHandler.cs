using SoulboundEngine.Client.World;
using SoulboundEngine.Client.World.Level;

namespace SoulboundEngine.World.Block {
	public interface INeighborUpdateHandler {
		void OnNeighborChanged(Level level, BlockPos selfPos, BlockPos neighborPos);
	}
}
