namespace SoulboundEngine.UnityClient.UI.Screen {
	using SoulboundEngine.Common;
	using SoulboundEngine.Registry;
	using SoulboundEngine.UnityClient.Assets;
	using SoulboundEngine.UnityClient.UI.UXMLBindings;
	using UnityEngine.UIElements;

	[PROTOTYPICAL]
	public sealed class GamePausedScreen : UXMLScreen {
		private static readonly Identifier RESUME_ELEMENT = Identifier.Of("soulbound:game_paused_screen/resume");
		private static readonly Identifier QUIT_ELEMENT = Identifier.Of("soulbound:game_paused_screen/quit");
		private readonly SoulboundUnityClient client;

		public GamePausedScreen(SoulboundUnityClient client) 
			: base(AssetManager.Resolve<VisualTreeAsset>(new AssetKey("GamePausedScreen"))) {
			this.client = client;
		}

		public override bool IsOpaque => false;
		public override bool CloseOnEsc => false;

		protected override void OnBind(VisualElement root) {
			root.Get<Button>(RESUME_ELEMENT).clicked += this.client.UnpauseGame;
			root.Get<Button>(QUIT_ELEMENT).clicked += this.client.QuitActiveWorld;
		}

	}
}
