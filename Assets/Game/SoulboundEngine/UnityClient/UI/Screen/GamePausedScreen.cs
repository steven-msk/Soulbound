namespace SoulboundEngine.UnityClient.UI.Screen {
	using SoulboundEngine.Common;
	using SoulboundEngine.UnityClient.Assets;
	using SoulboundEngine.UnityClient.UI.UXMLBindings;
	using UnityEngine.UIElements;

	[PROTOTYPICAL]
	public sealed class GamePausedScreen : UXMLScreen {
		private static readonly UXMLBinding<Button> RESUME_ELEMENT = new("soulbound:game_paused_screen/resume");
		private static readonly UXMLBinding<Button> QUIT_ELEMENT = new("soulbound:game_paused_screen/quit");
		private readonly SoulboundUnityClient client;

		public GamePausedScreen(SoulboundUnityClient client) 
			: base(AssetManager.Resolve<VisualTreeAsset>(new AssetKey("GamePausedScreen"))) {
			this.client = client;
		}

		public override bool IsOpaque => false;
		public override bool CloseOnEsc => false;

		protected override void OnBind(VisualElement root) {
			RESUME_ELEMENT.Get(root).clicked += this.client.UnpauseGame;
			QUIT_ELEMENT.Get(root).clicked += this.client.QuitActiveWorld;
		}

	}
}
