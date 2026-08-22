namespace SoulboundEngine.World.Widget {
	using SoulboundEngine.Registry;
	using System;

	public abstract class WorldWidgetType {
		public static readonly WorldWidgetType<TextWidgetHandler.Context> TEXT = Register<TextWidgetHandler.Context>("text", TextWidgetHandler.Create);

		private static WorldWidgetType<TContext> Register<TContext>(string id, Func<WorldWidgetType<TContext>, TContext, WorldWidgetHandler<TContext>> factory) 
				where TContext : WorldWidgetContext {
			RegistryKey<WorldWidgetType> key = KeyOf(id);
			WorldWidgetType<TContext> type = new(key, factory);
			return Registry<WorldWidgetType>.Register(Registries.WORLD_WIDGET_TYPE, key, type);
		}

		private static RegistryKey<WorldWidgetType> KeyOf(string id) {
			return RegistryKey<WorldWidgetType>.Of(Registries.WORLD_WIDGET_TYPE.GetKey(), Identifier.Of(id));
		}

		public static void Init() {
		}

		public RegistryKey<WorldWidgetType> key { get; private set; }

		protected WorldWidgetType(RegistryKey<WorldWidgetType> key) {
			this.key = key;
		}

		public abstract WorldWidgetHandler Instantiate(WorldWidgetContext context);
	}

	public sealed class WorldWidgetType<TContext> : WorldWidgetType where TContext : WorldWidgetContext {
		private readonly Func<WorldWidgetType<TContext>, TContext, WorldWidgetHandler<TContext>> factory;

		public WorldWidgetType(RegistryKey<WorldWidgetType> key, Func<WorldWidgetType<TContext>, TContext, WorldWidgetHandler<TContext>> factory) 
			: base(key) {
			this.factory = factory;
		}

		public override WorldWidgetHandler Instantiate(WorldWidgetContext context) {
			return this.factory(this, (TContext)context);
		}
	}
}
