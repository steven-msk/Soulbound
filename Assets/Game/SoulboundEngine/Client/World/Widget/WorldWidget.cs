using SoulboundEngine.Client.World.Block;
using UnityEngine;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.World.Widget {
	public abstract class WorldWidget {
		protected readonly VisualTreeAsset asset;
		protected readonly WorldWidgetContext creationContext;
		protected readonly WorldWidgetManager widgetManager;
		protected readonly UIDocument document;

		protected WorldWidget(WorldWidgetManager widgetManager, WorldWidgetContext context, VisualTreeAsset asset) {
			this.asset = asset;
			this.widgetManager = widgetManager;
			this.creationContext = context;

			this.document = widgetManager.CreateElement(asset);

			this.Bind(this.document, context);
		}

		public abstract void Update(WorldWidgetContext context);

		protected abstract void Bind(UIDocument uiDocument, WorldWidgetContext context);

		protected abstract Vector2 GetElementPos(UIDocument document, BlockPos blockPos);

		public void Destroy() {
			this.widgetManager.DestroyElement(this.document);
		}
	}

	public class WorldWidget<TContext> : WorldWidget where TContext : WorldWidgetContext {
		protected TContext context;

		public WorldWidget(WorldWidgetManager manager, TContext context, VisualTreeAsset asset) 
			: base(manager, context, asset) {
			this.context = context;
		}

		public sealed override void Update(WorldWidgetContext context) {
			this.context = (TContext)context;
			this.OnUpdate(this.context);
		}

		protected virtual void OnUpdate(TContext context) {
		}

		protected sealed override void Bind(UIDocument document, WorldWidgetContext context) {
			this.OnBind(document, (TContext)context);
		}

		protected virtual void OnBind(UIDocument document, TContext context) {
			Vector2 pos = this.GetElementPos(document, this.creationContext.blockPos);
			this.document.transform.position = pos;
		}

		protected override Vector2 GetElementPos(UIDocument document, BlockPos blockPos) {
			return blockPos.GetCenter();
		}
	}
}
