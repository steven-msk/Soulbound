namespace SoulboundEngine.World.Widget {
	using System.Collections.Generic;

	public class WorldWidgetManager {
		private readonly Dictionary<WorldWidgetHandle, WorldWidget> widgets = new();

		public WorldWidgetHandle ShowWidget<TContext>(WorldWidgetType<TContext> type, TContext context) where TContext : WorldWidgetContext {
			return this.ShowWidgetInternal(type, context);
		}

		private WorldWidgetHandle ShowWidgetInternal(WorldWidgetType type, WorldWidgetContext context) {
			WorldWidgetHandle handle = new(type, context);
			WorldWidget widget = type.Instantiate(context);
			this.widgets.Add(handle, widget);
			return handle;
		}

		public WorldWidget DestroyWidget(WorldWidgetHandle handle) {
			WorldWidget widget = this.widgets[handle];
			this.widgets.Remove(handle);
			widget.OnDestroy();
			return widget;
		}

		public void UpdateWidget<TContext>(WorldWidgetHandle handle, TContext context) where TContext : WorldWidgetContext {
			if (this.widgets.TryGetValue(handle, out WorldWidget widget)) {
				widget.Update(context);
				handle.UpdateContext(context);
			}
		}

		public void Clear() {
			foreach (WorldWidgetHandle handle in this.widgets.Keys) {
				this.DestroyWidget(handle);
			}
		}

		public WorldWidget Get(WorldWidgetHandle handle) => this.widgets[handle];

		public int Count() => this.widgets.Count;
	}
}
