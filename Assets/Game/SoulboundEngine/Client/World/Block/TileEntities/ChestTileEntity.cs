using SoulboundEngine.Client.Item.Container;

namespace SoulboundEngine.Client.World.Block.TileEntity {
	public class ChestTileEntity : TileEntity {
		private static readonly IInventoryLayout LAYOUT = new GridWrapInventoryLayout(9);
		private readonly IInventory inventory;

		public ChestTileEntity(TileEntityType<ChestTileEntity> tileEntityType, IInventory inventory, Level.Level level, BlockPos blockPos) 
			: base(tileEntityType, level, blockPos) {
			this.inventory = inventory;
		}

		public IInventory GetInventory() => this.inventory;

		public IInventoryLayout GetInventoryLayout() => LAYOUT;
	}
}
