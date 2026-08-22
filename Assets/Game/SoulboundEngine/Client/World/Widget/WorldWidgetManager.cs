namespace SoulboundEngine.Client.World.Widget {
	using SoulboundEngine.World.Level;
	using SoulboundEngine.World.Widget;
	using System;
	using System.Collections.Generic;

#nullable enable

	public class WorldWidgetManager {
		private Level? level;
		private readonly Dictionary<WorldWidgetHandler, WorldWidgetRenderer> renderedWidgets = new();
		private readonly Func<WorldWidgetHandler, WorldWidgetRenderer?> rendererFactory;

		public WorldWidgetManager(IEnumerable<WorldWidgetType> widgetTypes) {
			this.rendererFactory = WorldWidgetRenderers.GetFactory(widgetTypes);
		}

		public void SetLevel(Level? level) {
			if (this.level != null) {
				this.level.widgetAdded -= this.AddWidget;
				this.level.widgetRemoved -= this.RemoveWidget;
				foreach (WorldWidgetHandler handler in this.level.GetAllWidgets()) {
					this.RemoveWidget(handler);
				}

			}
			this.level = level;
			if (this.level != null) {
				this.level.widgetAdded += this.AddWidget;
				this.level.widgetRemoved += this.RemoveWidget;
				foreach (WorldWidgetHandler handler in this.level.GetAllWidgets()) {
					this.AddWidget(handler);
				}
			}
		}

		public void AddWidget(WorldWidgetHandler handler) {
			WorldWidgetRenderer? renderer = this.CreateRenderer(handler);
			if (renderer == null) return;
			this.renderedWidgets.Add(handler, renderer);
			renderer.Init();
		}

		public void RemoveWidget(WorldWidgetHandler handler) {
			if (this.renderedWidgets.Remove(handler, out WorldWidgetRenderer renderer)) {
				renderer.Destroy();
			}
		}

		private WorldWidgetRenderer? CreateRenderer(WorldWidgetHandler handler) {
			return this.rendererFactory(handler);
		}

		public WorldWidgetRenderer? GetRenderer(WorldWidgetHandler handler) {
			return this.renderedWidgets.GetValueOrDefault(handler);
		}
	}
}
