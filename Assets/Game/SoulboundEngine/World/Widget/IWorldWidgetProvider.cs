namespace SoulboundEngine.World.Widget {
	public interface IWorldWidgetProvider<TContext> where TContext : WorldWidgetContext {
		WorldWidgetHandler<TContext> CreateHandler(TContext context);
	}
}
