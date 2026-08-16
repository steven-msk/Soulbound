using SoulboundEngine.Client.UI.UXMLBindings;
using SoulboundEngine.Common.Math;
using SoulboundEngine.Registry;
using SoulboundEngine.World.Block;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.World.Widget {
	public class TextWidget : WorldWidget<TextWidget.Context> {
		private static readonly Identifier TEXT_ELEMENT = Identifier.Of("soulbound:text_widget/text");

		public TextWidget(WorldWidgetManager widgetManager, Context context, VisualTreeAsset asset) 
			: base(widgetManager, context, asset) {
		}

		protected override void OnBind(UIDocument document, Context context) {
			base.OnBind(document, context);
			this.SetText(context.text);
		}

		protected override Vec2d GetElementPos(UIDocument document, BlockPos blockPos) {
			Vec2d blockCenter = blockPos.GetCenter();
			return blockCenter + Vec2d.UNIT_Y;
		}

		protected override void OnUpdate(Context context) {
			this.SetText(context.text);
		}

		public void SetText(string text) {
			Label label = this.document.rootVisualElement.Get<Label>(TEXT_ELEMENT);
			label.text = text;
		}

		public class Context : WorldWidgetContext {
			public string text;
		}
	}
}
