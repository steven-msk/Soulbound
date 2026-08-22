namespace SoulboundEngine.Client.World.Widget {
	using SoulboundEngine.Client.Assets;
	using SoulboundEngine.World.Widget;
	using System;
	using System.Collections.Generic;
	using UnityEngine.UIElements;

#nullable enable

	public static class WorldWidgetRenderers {
		private static readonly Dictionary<WorldWidgetType, Func<WorldWidgetHandler, WorldWidgetRenderer>> RENDERER_FACTORY_BY_TYPE = new();
		private static Dictionary<WorldWidgetType, Func<WorldWidgetHandler, WorldWidgetRenderer>> CACHED_FACTORIES = new();

		static WorldWidgetRenderers() {
			Register<TextWidgetHandler, TextWidgetHandler.Context>(WorldWidgetType.TEXT,
				h => new TextWidgetRenderer(h, AssetManager.Resolve<VisualTreeAsset>(new AssetKey("TextWidget"))));
		}

		private static void Register<THandler, TContext>(WorldWidgetType type, Func<THandler, WorldWidgetRenderer<THandler, TContext>> factory)
				where THandler : WorldWidgetHandler<TContext> where TContext : WorldWidgetContext {
			RENDERER_FACTORY_BY_TYPE.Add(type, h => factory((THandler)h));
		}

		public static Func<WorldWidgetHandler, WorldWidgetRenderer?> GetFactory(IEnumerable<WorldWidgetType> types) {
			CACHED_FACTORIES = new();
			foreach (WorldWidgetType type in types) {
				if (!RENDERER_FACTORY_BY_TYPE.TryGetValue(type, out Func<WorldWidgetHandler, WorldWidgetRenderer> factory)) {
					Logger.LogError("Missing world widget renderer factory for type {}", type);
					continue;
				}
				CACHED_FACTORIES.Add(type, factory);
			}
			return handler => CACHED_FACTORIES.GetValueOrDefault(handler.GetWidgetType())?.Invoke(handler);
		}
	}
}
