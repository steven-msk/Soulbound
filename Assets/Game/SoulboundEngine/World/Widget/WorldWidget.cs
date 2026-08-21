namespace SoulboundEngine.World.Widget {
	public abstract class WorldWidget {
		private WorldWidgetContext context;

		protected WorldWidget(WorldWidgetContext context) {
			this.context = context;
		}

		public void Update(WorldWidgetContext context) {
			WorldWidgetContext oldContext = this.context;
			this.context = context;
			this.OnUpdate(oldContext, context);
		}

		protected abstract void OnUpdate(WorldWidgetContext oldContext, WorldWidgetContext newContext);

		public virtual void OnDestroy() {
		}
	}

	public abstract class WorldWidget<TContext> : WorldWidget where TContext : WorldWidgetContext {
		private TContext context;

		public WorldWidget(TContext context)
			: base(context) {
		}

		protected sealed override void OnUpdate(WorldWidgetContext oldContext, WorldWidgetContext newContext) {
			TContext typedOld = (TContext)oldContext;
			TContext typedNew = (TContext)newContext;
			this.context = typedNew;
			this.OnUpdate(typedOld, typedNew);
		}

		protected abstract void OnUpdate(TContext oldContext, TContext newContext);

		protected TContext GetContext() => this.context;
	}
}
