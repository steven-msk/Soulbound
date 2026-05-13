using SoulboundEngine.Client.World;
using SoulboundEngine.Core.Assets;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI.Screen {
	public class TitleScreen : Screen {
		private readonly IWorldAccessor worldAccessor;
		private readonly VisualTreeAsset asset;
		private readonly PanelSettings panelSettings;

		public TitleScreen(IWorldAccessor worldAccessor) {
			this.worldAccessor = worldAccessor;

			// TODO: fix resource leak from UI
			this.asset = AssetManager.Resolve<VisualTreeAsset>(new AssetKey("TitleScreen"));
			this.panelSettings = AssetManager.Resolve<PanelSettings>(new AssetKey("PanelSettings"));
		}

		//public override IScreenObject BuildObject(IScreenObjectFactory objFactory) {
		//	GameObject obj = new("Title Screen");

		//	UIDocument document = obj.AddComponent<UIDocument>();
		//	document.panelSettings = this.panelSettings;
		//	document.visualTreeAsset = this.asset;

		//	Button playButton = document.rootVisualElement.Q<Button>("PlayButton");
		//	Button exitButton = document.rootVisualElement.Q<Button>("ExitButton");

		//	// TODO: fix button events not available after the object's been hidden
		//	playButton.clicked += () => this.screenManager.PushScreen(new WorldListScreen(this.worldAccessor));
		//	exitButton.clicked += Soulbound.Instance.CloseGame;

		//	IScreenObject screenObj = objFactory.CreateSceneObject(this, obj);
		//	return screenObj;
		//}

		public override bool ReturnWithEscape => false;

		protected override void OnBuild(IScreenHandle handle) {
		}
	}
}
