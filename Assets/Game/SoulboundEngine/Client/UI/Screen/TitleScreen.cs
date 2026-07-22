using SoulboundEngine.Client.UI.UXMLBindings;
using SoulboundEngine.Client.World;
using SoulboundEngine.Core;
using SoulboundEngine.Core.Assets;
using SoulboundEngine.Core.Registry;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI.Screen {
	public class TitleScreen : UxmlScreen {
		private static readonly Identifier PLAY_BUTTON_ELEMENT = Identifier.Of("soulbound:title_screen/play_button");
		private static readonly Identifier EXIT_BUTTON_ELEMENT = Identifier.Of("soulbound:title_screen/exit_button");
		private readonly IWorldAccessor worldAccessor;

		// TODO: fix resource leak from UI
		public TitleScreen(IWorldAccessor worldAccessor)
			: base(AssetManager.Resolve<VisualTreeAsset>(new AssetKey("TitleScreen"))) {
			this.worldAccessor = worldAccessor;
		}

		public override bool CloseOnEsc => false;

		protected override void OnBind(VisualElement root) {
			root.Get<Button>(PLAY_BUTTON_ELEMENT).clicked += () => this.ScreenManager.PushScreen(new WorldListScreen(this.worldAccessor));
			root.Get<Button>(EXIT_BUTTON_ELEMENT).clicked += Soulbound.Instance.CloseGame;
		}
	}
}
