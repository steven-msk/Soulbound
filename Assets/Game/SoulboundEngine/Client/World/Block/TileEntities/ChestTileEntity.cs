using SoulboundEngine.Client.Item.Container;
using SoulboundEngine.Client.World.Level;

namespace SoulboundEngine.Client.World.Block.TileEntity {
	public class ChestTileEntity : TileEntity {
		private static readonly IInventoryLayout LAYOUT = new GridWrapInventoryLayout(9);
		private readonly Inventory inventory;

		public ChestTileEntity(TileEntityType<ChestTileEntity> tileEntityType, Inventory inventory, Level.Level level, BlockPos blockPos) 
			: base(tileEntityType, level, blockPos) {
			this.inventory = inventory;
		}

		public Inventory GetInventory() => this.inventory;

		public IInventoryLayout GetInventoryLayout() => LAYOUT;
	}
}
