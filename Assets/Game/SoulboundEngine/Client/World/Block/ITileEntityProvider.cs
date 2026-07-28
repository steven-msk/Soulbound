using SoulboundEngine.Client.World.Block.Entity;
using SoulboundEngine.Client.World.Block.State;

#nullable enable

namespace SoulboundEngine.Client.World.Block {

	public interface ITileEntityProvider {
		TileEntity? CreateTileEntity(BlockPos pos, BlockState state);
	}
}
