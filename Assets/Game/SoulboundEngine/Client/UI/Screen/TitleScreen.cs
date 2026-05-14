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

		protected override void OnBind(VisualElement root) {
			root.Q<Button>("PlayButton").clicked += () => this.screenManager.PushScreen(new WorldListScreen(this.worldAccessor));
			root.Q<Button>("ExitButton").clicked += Soulbound.Instance.CloseGame;
		}
	}
}
