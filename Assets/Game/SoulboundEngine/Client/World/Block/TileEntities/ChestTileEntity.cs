using SoulboundEngine.Client.ItemSystem.Container;
using SoulboundEngine.Client.World.LevelDomain;

namespace SoulboundEngine.Client.World.BlockSystem.TileEntities {
	public class ChestTileEntity : TileEntity {
		private readonly Inventory inventory;

		public ChestTileEntity(Inventory inventory, Level level, BlockPos blockPos) 
			: base(level, blockPos) {
			this.inventory = inventory;
		}

		public Inventory GetInventory() => this.inventory;
	}
}
