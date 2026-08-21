namespace SoulboundEngine.World.Widget {
	using SoulboundEngine.Registry;
	using System;

	public abstract partial class WorldWidgetType {
		public RegistryKey<WorldWidgetType> key { get; protected set; }

		public abstract WorldWidget Instantiate(WorldWidgetContext context);
	}

	public sealed class WorldWidgetType<TContext> : WorldWidgetType where TContext : WorldWidgetContext {
		private readonly Func<TContext, WorldWidget> factory;

		public WorldWidgetType(RegistryKey<WorldWidgetType> key, Func<TContext, WorldWidget> factory) {
			this.factory = factory;
			this.key = key;
		}

		public override WorldWidget Instantiate(WorldWidgetContext context) {
			return this.factory((TContext)context);
		}
	}
}
