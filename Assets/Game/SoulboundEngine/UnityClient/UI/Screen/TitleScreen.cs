namespace SoulboundEngine.UnityClient.UI.Screen {
	using SoulboundEngine.UnityClient.Assets;
	using SoulboundEngine.UnityClient.UI.UXMLBindings;
	using SoulboundEngine.World;
	using UnityEngine.UIElements;

	public class TitleScreen : UXMLScreen {
		private static readonly UXMLBinding<Button> PLAY_BUTTON_ELEMENT = new("soulbound:title_screen/play_button");
		private static readonly UXMLBinding<Button> EXIT_BUTTON_ELEMENT = new("soulbound:title_screen/exit_button");
		private readonly IWorldAccessor worldAccessor;

		// TODO: fix resource leak from UI
		public TitleScreen(IWorldAccessor worldAccessor)
			: base(AssetManager.Resolve<VisualTreeAsset>(new AssetKey("TitleScreen"))) {
			this.worldAccessor = worldAccessor;
		}

		public override bool CloseOnEsc => false;

		protected override void OnBind(VisualElement root) {
			PLAY_BUTTON_ELEMENT.Get(root).clicked += () => this.ScreenManager.PushScreen(new WorldListScreen(this.worldAccessor));
			EXIT_BUTTON_ELEMENT.Get(root).clicked += SoulboundUnityClient.Instance.Close;
		}
	}
}
