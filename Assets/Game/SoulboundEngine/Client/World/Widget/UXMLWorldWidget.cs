namespace SoulboundEngine.Client.World.Widget {
	using SoulboundEngine.Common.Math;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Widget;
	using UnityEngine;
	using UnityEngine.UIElements;

	public abstract class UXMLWorldWidget<TContext> : WorldWidget<TContext> where TContext : WorldWidgetContext {
		protected readonly VisualTreeAsset asset;
		protected readonly TContext creationContext;
		protected readonly ClientWorldWidgetManager widgetManager;
		protected readonly UIDocument document;

		protected UXMLWorldWidget(ClientWorldWidgetManager widgetManager, TContext context, VisualTreeAsset asset)
			: base(context) {
			this.asset = asset;
			this.widgetManager = widgetManager;
			this.creationContext = context;

			this.document = widgetManager.CreateElement(asset);

			this.Bind(this.document, context);
		}

		public virtual void Update(TContext context) {
		}

		protected virtual void Bind(UIDocument document, TContext context) {
			Vec2d pos = this.GetElementPos(document, this.creationContext.blockPos);
			this.document.transform.position = new Vector2((float)pos.x, (float)pos.y);
		}

		protected virtual Vec2d GetElementPos(UIDocument document, BlockPos blockPos) {
			return blockPos.GetCenter();
		}

		public override void OnDestroy() {
			this.widgetManager.DestroyElement(this.document);
		}
	}
}
