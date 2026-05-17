using SoulboundEngine.Client.ItemSystem.Container;
using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Client.World.BlockSystem.TileEntities;
using SoulboundEngine.Client.World.LevelDomain;

namespace SoulboundEngine.Client.World.BlockSystem {
	public class ChestBlock : Block {
		public const int INVENTORY_SIZE = 9;

		public ChestBlock(Settings settings)
			: base(settings) {
		}

		public override bool HasTileEntity(Level level, BlockPos blockPos, BlockState blockState) {
			return true;
		}

		public override TileEntity GetTileEntity(Level level, BlockPos blockPos) {
			return new ChestTileEntity(this.GetInventory(), level, blockPos);
		}

		private Inventory GetInventory() => new(INVENTORY_SIZE);
	}
}
