namespace SoulboundEngine.World.Block {
	using SoulboundEngine.World.Block.Entity;
	using SoulboundEngine.World.Block.State;

#nullable enable

	public interface ITileEntityProvider {
		TileEntity? CreateTileEntity(BlockPos pos, BlockState state);
	}
}
