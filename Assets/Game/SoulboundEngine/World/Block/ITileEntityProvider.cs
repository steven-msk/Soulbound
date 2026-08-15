using SoulboundEngine.Client.World;
using SoulboundEngine.World.Block.Entity;
using SoulboundEngine.World.Block.State;

#nullable enable

namespace SoulboundEngine.World.Block {

	public interface ITileEntityProvider {
		TileEntity? CreateTileEntity(BlockPos pos, BlockState state);
	}
}
