using SoulboundEngine.Client.Loot;
using SoulboundEngine.Client.Player;
using SoulboundEngine.Interaction;
using SoulboundEngine.Item.Container;
using SoulboundEngine.World.Block.Entity;
using SoulboundEngine.World.Block.State;
using SoulboundEngine.World.Entity;

namespace SoulboundEngine.World.Block {
	using Level = Level.Level;

	public class ChestBlock : Block, ITileEntityProvider {
		public const int INVENTORY_SIZE = 27;

		public ChestBlock(AbstractBlock.Settings settings)
			: base(settings) {
		}

		protected override IActionResult OnSecondaryUse(BlockState state, Level level, PlayerEntity player, BlockPos pos) {
			ChestTileEntity chestTileEntity = (ChestTileEntity)level.GetTileEntity(pos);
			player.OpenInventoryScreen(chestTileEntity);
			chestTileEntity.OnOpened(player);
			return IActionResult.SUCCESS;
		}

		protected override void OnStateReplaced(BlockState state, BlockPos pos, Level level) {
			ChestTileEntity chestTileEntity = (ChestTileEntity)level.GetTileEntity(pos);
			foreach (var stack in chestTileEntity) {
				ItemEntity itemEntity = new(stack, level);
				itemEntity.SetPosition(pos.GetCenter());
				level.AddEntity(itemEntity);
			}
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
