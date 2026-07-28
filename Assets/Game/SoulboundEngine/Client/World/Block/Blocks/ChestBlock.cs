using SoulboundEngine.Client.Interaction;
using SoulboundEngine.Client.World.Block.Entity;
using SoulboundEngine.Client.World.Block.State;

namespace SoulboundEngine.Client.World.Block {
	public class ChestBlock : Block, IInteractableBlock, ITileEntityProvider {
		public const int INVENTORY_SIZE = 27;

		public ChestBlock(Settings settings)
			: base(settings) {
		}

		public bool CanInteract(in BlockInteraction ctx) => true;

		public bool ValidateTrigger(InteractionTrigger trigger) {
			return trigger is InteractionTrigger.RightClick;
		}

		public void OnInteract(in BlockInteraction ctx) {
			ChestTileEntity chestTileEntity = (ChestTileEntity)ctx.level.GetTileEntity(ctx.blockPos);
			ctx.player.OpenInventoryScreen(chestTileEntity);
			chestTileEntity.OnOpened(ctx.player);
		}

		public TileEntity CreateTileEntity(BlockPos pos, BlockState state) {
			return ChestTileEntity.Create(pos, state);
		}
	}
}
