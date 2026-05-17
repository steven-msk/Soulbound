using SoulboundEngine.Client.Interaction;
using SoulboundEngine.Client.ItemSystem.Container;
using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Client.World.BlockSystem.TileEntities;
using SoulboundEngine.Client.World.LevelDomain;

namespace SoulboundEngine.Client.World.BlockSystem {
	public class ChestBlock : Block, IInteractableBlock {
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

		public bool CanInteract(in BlockInteraction ctx) => true;

		public bool ValidateTrigger(InteractionTrigger trigger) {
			return trigger is InteractionTrigger.RightClick;
		}

		public void OnInteract(in BlockInteraction ctx) {
			ChestTileEntity chestTileEntity = (ChestTileEntity)ctx.level.GetTileEntity(ctx.blockPos);
			ctx.player?.OpenInventory(chestTileEntity.GetInventory(), chestTileEntity);
		}
	}
}
