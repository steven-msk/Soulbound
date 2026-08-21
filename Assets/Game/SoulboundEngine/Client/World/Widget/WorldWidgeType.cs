namespace SoulboundEngine.World.Widget {
	using SoulboundEngine.Client.Assets;
	using SoulboundEngine.Client.World.Widget;
	using SoulboundEngine.Registry;
	using System;
	using UnityEngine.UIElements;

	// TODO: rework world widget type registration
	// maybe change to RegistryKey<WorldWidget> so that they can be defined in core

	public abstract partial class WorldWidgetType {
		public static readonly WorldWidgetType<TextWidget.Context> TEXT = Register<TextWidget, TextWidget.Context>("text",
			(manager, context) => new TextWidget(manager, context, AssetManager.Resolve<VisualTreeAsset>(new AssetKey("TextWidget")))
		);

		private static WorldWidgetType<TContext> Register<TWidget, TContext>(string id, Func<ClientWorldWidgetManager, TContext, TWidget> factory)
				where TWidget : WorldWidget<TContext> where TContext : WorldWidgetContext {
			RegistryKey<WorldWidgetType> key = KeyOf(id);
			WorldWidgetType<TContext> type = new(key, context => factory(ClientWorldWidgetManager.Instance, context));
			return Registry<WorldWidgetType>.Register(Registries.WORLD_WIDGET_TYPE, key, type);
		}

		private static RegistryKey<WorldWidgetType> KeyOf(string id) {
			return RegistryKey<WorldWidgetType>.Of(Registries.WORLD_WIDGET_TYPE.GetKey(), Identifier.Of(id));
		}

		public static void Init() {
		}
	}
}
