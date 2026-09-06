namespace SoulboundEngine.UnityClient.World.Widget {
	using SoulboundEngine.Common.Math;
	using SoulboundEngine.UnityClient.Assets;
	using SoulboundEngine.UnityClient.UI.UXMLBindings;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Widget;
	using UnityEngine;
	using UnityEngine.UIElements;

	public class TextWidgetRenderer : WorldWidgetRenderer<TextWidgetHandler, TextWidgetHandler.Context> {
		private static readonly UXMLBinding<Label> TEXT_ELEMENT = new("soulbound:text_widget/text");
		private readonly TextWidgetHandler.Context creationContext;
		private readonly VisualTreeAsset asset;
		private UIDocument document;

		public TextWidgetRenderer(TextWidgetHandler handler, VisualTreeAsset asset) 
			: base(handler) {
			this.creationContext = handler.GetCurrentContext();
			this.asset = asset;
		}

		public override void Init() {
			this.document = CreateElement(this.asset);
			this.Bind(this.creationContext);
		}

		private void Bind(TextWidgetHandler.Context context) {
			Vec2d pos = this.GetElementPos(context.blockPos);
			this.document.transform.position = new Vector2((float)pos.x, (float)pos.y);
			this.SetText(context.text);
		}

		private Vec2d GetElementPos(BlockPos blockPos) {
			Vec2d blockCenter = blockPos.GetCenter();
			return blockCenter + Vec2d.UNIT_Y;
		}

		protected override void ContextUpdate(TextWidgetHandler.Context oldContext, TextWidgetHandler.Context newContext) {
			this.SetText(newContext.text);
		}

		public void SetText(string text) {
			Label label = TEXT_ELEMENT.Get(this.document.rootVisualElement);
			label.text = text;
		}

		public override void Destroy() {
			DestroyElement(this.document);
		}

		public static UIDocument CreateElement(VisualTreeAsset asset) {
			GameObject prefab = AssetManager.Resolve<GameObject>(new AssetKey("WorldWidgetDocument"));
			GameObject obj = GameObject.Instantiate(prefab);

			UIDocument document = obj.GetComponent<UIDocument>();
			document.visualTreeAsset = asset;

			return document;
		}

		public static void DestroyElement(UIDocument element) {
			GameObject.Destroy(element.gameObject);
		}

	}
}
