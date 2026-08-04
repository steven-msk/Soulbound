using SoulboundEngine.Client.UI.UXMLBindings;
using SoulboundEngine.Client.World.Block;
using SoulboundEngine.Core.Registry;
using UnityEngine;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.World.Widget {
	public class SignWidget : WorldWidget<SignWidget.Context> {
		private static readonly Identifier TEXT_ELEMENT = Identifier.Of("soulbound:sign_text/text");

		public SignWidget(WorldWidgetManager widgetManager, Context context, VisualTreeAsset asset) 
			: base(widgetManager, context, asset) {
		}

		protected override void OnBind(UIDocument document, Context context) {
			document.rootVisualElement.RegisterCallback<GeometryChangedEvent>(evt => {
				Vector2 pos = this.GetElementPos(document, this.creationContext.blockPos);
				document.transform.position = pos;
			});
			Label label = document.rootVisualElement.Get<Label>(TEXT_ELEMENT);
			label.text = context.text;
		}

		protected override Vector2 GetElementPos(UIDocument document, BlockPos blockPos) {
			Vector2 blockCenter = blockPos.GetCenter();
			return blockCenter + Vector2.up;
		}

		public class Context : WorldWidgetContext {
			public string text;
		}
	}
}
