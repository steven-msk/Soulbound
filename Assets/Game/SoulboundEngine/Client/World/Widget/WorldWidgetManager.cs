using SoulboundEngine.Core.Assets;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.World.Widget {
	public class WorldWidgetManager {
		private readonly Dictionary<WorldWidgetHandle, WorldWidget> widgets = new();
		private readonly Dictionary<UIDocument, GameObject> elements = new();

		public WorldWidgetHandle ShowWidget<TContext>(WorldWidgetType<TContext> type, TContext context) where TContext : WorldWidgetContext {
			return this.ShowWidgetUnsafe(type, context);
		}

		public WorldWidgetHandle ShowWidgetUnsafe(WorldWidgetType type, WorldWidgetContext context) {
			WorldWidgetHandle handle = new(type, context);
			WorldWidget widget = type.Instantiate(this, context);
			this.widgets.Add(handle, widget);
			return handle;
		}

		public WorldWidget DestroyWidget(WorldWidgetHandle handle) {
			WorldWidget widget = this.widgets[handle];
			this.widgets.Remove(handle);
			widget.Destroy();
			return widget;
		}

		public void UpdateWidget<TContext>(WorldWidgetHandle handle, TContext context) where TContext : WorldWidgetContext {
			if (this.widgets.TryGetValue(handle, out WorldWidget widget)) {
				widget.Update(context);
				handle.UpdateContext(context);
			}
		}

		public void Clear() {
			foreach (var handle in this.widgets.Keys) {
				this.DestroyWidget(handle);
			}
		}

		public WorldWidget Get(WorldWidgetHandle handle) => this.widgets[handle];

		public int Count() => this.widgets.Count;

		public UIDocument CreateElement(VisualTreeAsset asset) {
			GameObject prefab = AssetManager.Resolve<GameObject>(new AssetKey("WorldWidgetDocument"));
			GameObject obj = GameObject.Instantiate(prefab);

			UIDocument document = obj.GetComponent<UIDocument>();
			document.visualTreeAsset = asset;

			this.elements.Add(document, obj);
			return document;
		}

		public void DestroyElement(UIDocument element) {
			if (!this.elements.Remove(element, out GameObject obj)) return;
			GameObject.Destroy(obj);
		}
	}
}
