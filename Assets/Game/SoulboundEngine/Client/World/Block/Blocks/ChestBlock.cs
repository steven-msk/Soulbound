using SoulboundEngine.Client.Interaction;
using SoulboundEngine.Client.World.Block.State;
using SoulboundEngine.Client.World.Block.TileEntity;

namespace SoulboundEngine.Client.World.Block {
	public class ChestBlock : Block, IInteractableBlock {
		public const int INVENTORY_SIZE = 27;

		public ChestBlock(Settings settings)
			: base(settings) {
		}

		public override bool HasTileEntity(Level.Level level, BlockPos blockPos, BlockState blockState) {
			return true;
		}

		public override TileEntity.TileEntity GetTileEntity(Level.Level level, BlockPos blockPos) {
			return new ChestTileEntity(TileEntityTypes.CHEST, level, blockPos);
		}

		public bool CanInteract(in BlockInteraction ctx) => true;

		public bool ValidateTrigger(InteractionTrigger trigger) {
			return trigger is InteractionTrigger.RightClick;
		}

		public void OnInteract(in BlockInteraction ctx) {
			ChestTileEntity chestTileEntity = (ChestTileEntity)ctx.level.GetTileEntity(ctx.blockPos);
		}
	}
}
