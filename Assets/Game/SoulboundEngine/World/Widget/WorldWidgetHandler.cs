namespace SoulboundEngine.World.Widget {
	using System;

#nullable enable

	public abstract class WorldWidgetHandler {
		protected readonly WorldWidgetType widgetType;
		public abstract event Action<WorldWidgetContext, WorldWidgetContext>? contextUpdate;

		public WorldWidgetHandler(WorldWidgetType widgetType) {
			this.widgetType = widgetType;
		}

		public abstract void Init(WorldWidgetContext context);
		public abstract void Update(WorldWidgetContext context);

		public abstract WorldWidgetContext GetContext();

		public WorldWidgetType GetWidgetType() => this.widgetType;
	}

	public abstract class WorldWidgetHandler<TContext> : WorldWidgetHandler where TContext : WorldWidgetContext {
		private TContext context;
		protected new readonly WorldWidgetType<TContext> widgetType;
		private bool initalized;
		public sealed override event Action<WorldWidgetContext, WorldWidgetContext>? contextUpdate;

		public WorldWidgetHandler(WorldWidgetType<TContext> widgetType, TContext context)
			: base(widgetType) {
			this.widgetType = widgetType;
			this.context = context;
		}

		public sealed override void Init(WorldWidgetContext context) {
			this.Init((TContext)context);
		}

		public void Init(TContext context) {
			if (this.initalized) throw new InvalidOperationException("World widget handler already initialized: " + this);
			this.initalized = true;
			this.OnInit(context);
		}

		protected abstract void OnInit(TContext context);

		public sealed override void Update(WorldWidgetContext context) {
			this.Update((TContext)context);
		}

		public void Update(TContext context) {
			TContext oldContext = this.context;
			if (oldContext.level != context.level) {
				throw new ArgumentException("Cannot change the level in the context of a world widget");
			}
			if (oldContext.blockPos != context.blockPos) {
				throw new ArgumentException("Cannot change the block pos in the context of a world widget");
			}

			this.context = context;
			this.OnUpdate(oldContext, this.context);
			this.contextUpdate?.Invoke(oldContext, context);
		}

		public sealed override WorldWidgetContext GetContext() => this.GetCurrentContext();

		public TContext GetCurrentContext() => this.context;

		protected abstract void OnUpdate(TContext oldContext, TContext context);
	}
}
