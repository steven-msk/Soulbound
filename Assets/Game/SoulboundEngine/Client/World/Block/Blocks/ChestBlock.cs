using SoulboundEngine.Client.Interaction;
using SoulboundEngine.Client.Item.Container;
using SoulboundEngine.Client.Loot;
using SoulboundEngine.Client.World.Block.Entity;
using SoulboundEngine.Client.World.Block.State;

namespace SoulboundEngine.Client.World.Block {
	public class ChestBlock : Block, IInteractableBlock, ITileEntityProvider {
		public const int INVENTORY_SIZE = 27;

		public ChestBlock(Settings settings)
			: base(settings) {
		}

		public bool CanInteract(in BlockInteractionResult ctx) => true;

		public bool ValidateTrigger(InteractionTrigger trigger) {
			return trigger is InteractionTrigger.RightClick;
		}

		public void OnInteract(in BlockInteractionResult ctx) {
			ChestTileEntity chestTileEntity = (ChestTileEntity)ctx.level.GetTileEntity(ctx.blockPos);
			ctx.player.OpenInventoryScreen(chestTileEntity);
			chestTileEntity.OnOpened(ctx.player);
		}

		public TileEntity CreateTileEntity(BlockPos pos, BlockState state) {
			// PROTOTYPICAL
			ChestTileEntity tileEntity = ChestTileEntity.Create(pos, state);
			static long Mix(long a, long b) {
				ulong x = unchecked((ulong)(a ^ b) + 0x9E3779B97F4A7C15UL);
				x = (x ^ (x >> 30)) * 0xBF58476D1CE4E5B9UL;
				x = (x ^ (x >> 27)) * 0x94D049BB133111EBUL;
				return unchecked((long)(x ^ (x >> 31)));
			}
			long chestSeed = Mix(143261890564893, pos.GetHashCode());
			tileEntity.SetLootTable(LootTables.CHEST_TEST, chestSeed);
			return tileEntity;
		}
	}
}
