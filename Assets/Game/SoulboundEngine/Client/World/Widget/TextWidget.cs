namespace SoulboundEngine.Client.World.Widget {
	using SoulboundEngine.Client.UI.UXMLBindings;
	using SoulboundEngine.Common.Math;
	using SoulboundEngine.Registry;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Widget;
	using UnityEngine.UIElements;

	public class TextWidget : UXMLWorldWidget<TextWidget.Context> {
		private static readonly Identifier TEXT_ELEMENT = Identifier.Of("soulbound:text_widget/text");

		public TextWidget(ClientWorldWidgetManager widgetManager, Context context, VisualTreeAsset asset)
			: base(widgetManager, context, asset) {
		}

		protected override void Bind(UIDocument document, Context context) {
			base.Bind(document, context);
			this.SetText(context.text);
		}

		protected override Vec2d GetElementPos(UIDocument document, BlockPos blockPos) {
			Vec2d blockCenter = blockPos.GetCenter();
			return blockCenter + Vec2d.UNIT_Y;
		}

		protected override void OnUpdate(Context oldContext, Context newContext) {
			this.SetText(newContext.text);
		}

		public void SetText(string text) {
			Label label = this.document.rootVisualElement.Get<Label>(TEXT_ELEMENT);
			label.text = text;
		}

		public record Context(string text, BlockPos blockPos) : WorldWidgetContext(blockPos) {
		}
	}
}
