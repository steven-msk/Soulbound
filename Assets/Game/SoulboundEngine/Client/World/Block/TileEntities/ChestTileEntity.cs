namespace SoulboundEngine.Client.World.Block.TileEntity {
	using Level = Level.Level;

	public class ChestTileEntity : TileEntity {
		public ChestTileEntity(TileEntityType<ChestTileEntity> tileEntityType, Level level, BlockPos blockPos) 
			: base(tileEntityType, level, blockPos) {
		}
	}
}
