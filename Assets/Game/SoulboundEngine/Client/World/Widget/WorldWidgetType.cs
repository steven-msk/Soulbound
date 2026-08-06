using SoulboundEngine.Core.Assets;
using SoulboundEngine.Core.Registry;
using System;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.World.Widget {
	public abstract class WorldWidgetType {
		public static readonly WorldWidgetType<TextWidget.Context> TEXT = Register<TextWidget, TextWidget.Context>("text",
			(manager, context) => new TextWidget(manager, context, AssetManager.Resolve<VisualTreeAsset>(new AssetKey("TextWidget")))
		);

		private static WorldWidgetType<TContext> Register<TWidget, TContext>(string id, Func<WorldWidgetManager, TContext, TWidget> factory)
				where TWidget : WorldWidget<TContext> where TContext : WorldWidgetContext {
			RegistryKey<WorldWidgetType> key = KeyOf(id);
			WorldWidgetType<TContext> type = new(key, factory);
			return Registry<WorldWidgetType>.Register(Registries.WORLD_WIDGET_TYPE, key, type);
		}

		private static RegistryKey<WorldWidgetType> KeyOf(string id) {
			return RegistryKey<WorldWidgetType>.Of(Registries.WORLD_WIDGET_TYPE.GetKey(), Identifier.Of(id));
		}

		public static void Init() {
		}

		public RegistryKey<WorldWidgetType> key { get; protected set; }

		public abstract WorldWidget Instantiate(WorldWidgetManager manager, WorldWidgetContext context);
	}

	public sealed class WorldWidgetType<TContext> : WorldWidgetType where TContext : WorldWidgetContext {
		private readonly Func<WorldWidgetManager, TContext, WorldWidget> factory;

		public WorldWidgetType(RegistryKey<WorldWidgetType> key, Func<WorldWidgetManager, TContext, WorldWidget> factory) {
			this.factory = factory;
			this.key = key;
		}

		public override WorldWidget Instantiate(WorldWidgetManager manager, WorldWidgetContext context) {
			return this.factory(manager, (TContext)context);
		}
	}
}
