using SoulboundEngine.World.Block;
using SoulboundEngine.World.Block.Entity;
using SoulboundEngine.World.Block.State;

#nullable enable

namespace SoulboundEngine.World {
	public interface IBlockGetter : IHeightLimitView {
		BlockState GetBlockState(BlockPos blockPos);
		TileEntity? GetTileEntity(BlockPos blockPos);
	}
}
