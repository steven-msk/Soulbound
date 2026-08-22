namespace SoulboundEngine.Client.World.Widget {
	using SoulboundEngine.World.Widget;

	public abstract class WorldWidgetRenderer {
		public WorldWidgetRenderer(WorldWidgetHandler handler) {
			handler.contextUpdate += this.ContextUpdate;
		}

		public abstract void Init();

		protected abstract void ContextUpdate(WorldWidgetContext oldContext, WorldWidgetContext newContext);

		public abstract void Destroy();

		public abstract WorldWidgetHandler GetHandler();
	}

	public abstract class WorldWidgetRenderer<THandler, TContext> : WorldWidgetRenderer where THandler : WorldWidgetHandler<TContext> where TContext : WorldWidgetContext {
		protected readonly THandler handler;

		public WorldWidgetRenderer(THandler handler) 
			: base(handler) {
			this.handler = handler;
		}

		public sealed override WorldWidgetHandler GetHandler() => this.handler;

		protected sealed override void ContextUpdate(WorldWidgetContext oldContext, WorldWidgetContext newContext) {
			this.ContextUpdate((TContext)oldContext, (TContext)newContext);
		}

		protected abstract void ContextUpdate(TContext oldContext, TContext newContext);
	}
}
