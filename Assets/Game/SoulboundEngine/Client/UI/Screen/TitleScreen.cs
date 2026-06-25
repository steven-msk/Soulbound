using SoulboundEngine.Client.World;
using SoulboundEngine.Core;
using SoulboundEngine.Core.Assets;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI.Screen {
	public class TitleScreen : UxmlScreen {
		private readonly IWorldAccessor worldAccessor;

		// TODO: fix resource leak from UI
		public TitleScreen(IWorldAccessor worldAccessor)
			: base(AssetManager.Resolve<VisualTreeAsset>(new AssetKey("TitleScreen"))) {
			this.worldAccessor = worldAccessor;
		}

		public override bool CloseOnEsc => false;

		protected override void OnBind(VisualElement root) {
			root.Q<Button>("PlayButton").clicked += () => this.ScreenManager.PushScreen(new WorldListScreen(this.worldAccessor));
			root.Q<Button>("ExitButton").clicked += Soulbound.Instance.CloseGame;
		}
	}
}
