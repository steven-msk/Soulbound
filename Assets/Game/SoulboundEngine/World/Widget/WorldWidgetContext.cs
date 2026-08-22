namespace SoulboundEngine.World.Widget {
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Level;

	public record WorldWidgetContext(Level level, BlockPos blockPos) {
	}
}
