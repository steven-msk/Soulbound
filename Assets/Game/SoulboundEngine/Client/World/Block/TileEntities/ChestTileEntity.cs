using SoulboundEngine.Client.Item.Container;
using SoulboundEngine.Client.Player;
using SoulboundEngine.Client.UI.Screen;

namespace SoulboundEngine.Client.World.Block.TileEntity {
	public class ChestTileEntity : TileEntity, IInventoryScreenHandlerFactory {
		private static readonly IInventoryLayout LAYOUT = new GridWrapInventoryLayout(9);
		private readonly Inventory inventory;

		public ChestTileEntity(TileEntityType<ChestTileEntity> tileEntityType, Inventory inventory, Level.Level level, BlockPos blockPos) 
			: base(tileEntityType, level, blockPos) {
			this.inventory = inventory;
		}

		public InventoryScreenHandler Create(PlayerInventory playerInventory, PlayerEntity player) {
			throw new System.NotImplementedException();
		}

		public Inventory GetInventory() => this.inventory;

		public IInventoryLayout GetInventoryLayout() => LAYOUT;
	}
}
