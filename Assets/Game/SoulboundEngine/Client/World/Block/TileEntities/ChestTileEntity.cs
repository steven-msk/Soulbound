using SoulboundEngine.Client.Player;
using SoulboundEngine.Client.UI.Screen;

namespace SoulboundEngine.Client.World.Block.TileEntity {
	using Level = Level.Level;

	public class ChestTileEntity : TileEntity, IInventoryScreenHandlerFactory {
		public ChestTileEntity(TileEntityType<ChestTileEntity> tileEntityType, Level level, BlockPos blockPos) 
			: base(tileEntityType, level, blockPos) {
		}

		public InventoryScreenHandler Create(PlayerInventory playerInventory, PlayerEntity player) {
			throw new System.NotImplementedException();
		}

	}
}
