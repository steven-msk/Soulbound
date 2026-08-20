using SoulboundEngine.Client.World.Widget;
using SoulboundEngine.Interaction;
using SoulboundEngine.Item;
using SoulboundEngine.World.Block.Entity;
using SoulboundEngine.World.Block.State;
using SoulboundEngine.World.Player;

namespace SoulboundEngine.World.Block {
	using Level = Level.Level;

	public class SignBlock : Block, ITileEntityProvider {
		public SignBlock(Settings settings) 
			: base(settings) {
		}

		public TileEntity CreateTileEntity(BlockPos pos, BlockState state) {
			return new SignTileEntity(pos, state);
		}

		protected override void OnHoverEnter(BlockState state, ItemStack stack, Level level, PlayerEntity player, BlockPos pos) {
			SignTileEntity tileEntity = (SignTileEntity)level.GetTileEntity(pos);
			tileEntity.widgetHandle = player.ShowWorldWidget(WorldWidgetType.TEXT, new TextWidget.Context() { 
				blockPos = pos, text = tileEntity.GetText()
			});
		}

		protected override void OnHoverLeave(BlockState state, ItemStack stack, Level level, PlayerEntity player, BlockPos pos) {
			SignTileEntity tileEntity = (SignTileEntity)level.GetTileEntity(pos);
			if (tileEntity.widgetHandle == null) return;
			player.DestroyWorldWidget(tileEntity.widgetHandle);
			tileEntity.widgetHandle = null;
		}

		protected override IActionResult OnSecondaryUse(BlockState state, Level level, PlayerEntity player, BlockPos pos) {
			SignTileEntity tileEntity = (SignTileEntity)level.GetTileEntity(pos);
			if (tileEntity.screenHandle != null) return IActionResult.FAIL;

			player.OpenSignEditScreen(tileEntity);
			return IActionResult.SUCCESS;
		}
	}
}
