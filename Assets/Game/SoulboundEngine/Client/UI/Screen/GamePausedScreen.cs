namespace SoulboundEngine.Client.UI.Screen {
	using SoulboundEngine.Client.Assets;
	using SoulboundEngine.Client.UI.UXMLBindings;
	using SoulboundEngine.Common;
	using SoulboundEngine.Registry;
	using UnityEngine.UIElements;

	[PROTOTYPICAL]
	public sealed class GamePausedScreen : UXMLScreen {
		private static readonly Identifier RESUME_ELEMENT = Identifier.Of("soulbound:game_paused_screen/resume");
		private static readonly Identifier QUIT_ELEMENT = Identifier.Of("soulbound:game_paused_screen/quit");
		private readonly SoulboundClient client;

		public GamePausedScreen(SoulboundClient client) 
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
