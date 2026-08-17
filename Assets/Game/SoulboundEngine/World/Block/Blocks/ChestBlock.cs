using SoulboundEngine.Common.Math;
using SoulboundEngine.Common.Math.Random;
using SoulboundEngine.Interaction;
using SoulboundEngine.Item.Container;
using SoulboundEngine.Loot;
using SoulboundEngine.World.Block.Entity;
using SoulboundEngine.World.Block.State;
using SoulboundEngine.World.Entity;
using SoulboundEngine.World.Player;

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
				Vec2d p = pos.GetCenter();
				ItemEntity itemEntity = new(level, p.x, p.y, stack);
				itemEntity.SetPos(pos.GetCenter());
				level.AddEntity(itemEntity);
			}
		}

		public TileEntity CreateTileEntity(BlockPos pos, BlockState state) {
			ChestTileEntity tileEntity = ChestTileEntity.Create(pos, state);

			// PROTOTYPICAL
			long chestSeed = RandomProvider.Mix(143261890564893, pos.GetHashCode());
			tileEntity.SetLootTable(LootTables.CHEST_TEST, chestSeed);

			return tileEntity;
		}
	}
}
