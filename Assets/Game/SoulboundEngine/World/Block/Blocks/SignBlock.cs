namespace SoulboundEngine.World.Block {
	using SoulboundEngine.Interaction;
	using SoulboundEngine.Item;
	using SoulboundEngine.World.Block.Entity;
	using SoulboundEngine.World.Block.State;
	using SoulboundEngine.World.Level;
	using SoulboundEngine.World.Player;
	using SoulboundEngine.World.Widget;

	public class SignBlock : Block, ITileEntityProvider, IWorldWidgetProvider<TextWidgetHandler.Context> {
		public SignBlock(Settings settings) 
			: base(settings) {
		}

		public WorldWidgetHandler<TextWidgetHandler.Context> CreateHandler(TextWidgetHandler.Context context) {
			return TextWidgetHandler.Create(WorldWidgetType.TEXT, context);
		}

		public TileEntity CreateTileEntity(BlockPos pos, BlockState state) {
			return new SignTileEntity(pos, state);
		}

		protected override void OnHoverEnter(BlockState state, ItemStack stack, Level level, PlayerEntity player, BlockPos pos) {
			SignTileEntity tileEntity = (SignTileEntity)level.GetTileEntity(pos);
			tileEntity.widgetHandler = level.AddWidget(this, (level, pos) => new TextWidgetHandler.Context(level, pos, tileEntity.GetText()), pos);
		}

		protected override void OnHoverLeave(BlockState state, ItemStack stack, Level level, PlayerEntity player, BlockPos pos) {
			SignTileEntity tileEntity = (SignTileEntity)level.GetTileEntity(pos);
			if (tileEntity.widgetHandler == null) return;

			level.RemoveWidget(tileEntity.widgetHandler);
			tileEntity.widgetHandler = null;
		}

		protected override IActionResult OnSecondaryUse(BlockState state, Level level, PlayerEntity player, BlockPos pos) {
			SignTileEntity tileEntity = (SignTileEntity)level.GetTileEntity(pos);
			if (tileEntity.screenHandle != null) return IActionResult.FAIL;

			player.OpenSignEditScreen(tileEntity);
			return IActionResult.SUCCESS;
		}
	}
}
