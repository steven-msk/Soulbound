using SoulboundEngine.Client.World.Block.Entity;
using SoulboundEngine.Client.World.Block.State;

#nullable enable

namespace SoulboundEngine.Client.World {
	public interface IBlockGetter : IHeightLimitView {
		BlockState GetBlockState(BlockPos blockPos);
		TileEntity? GetTileEntity(BlockPos blockPos);
	}
}
