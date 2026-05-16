using SoulboundEngine.Client.World;
using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Common;
using SoulboundEngine.Core.Assets;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI.Screen {
	[PROTOTYPICAL]
	public sealed class GamePausedScreen : UxmlScreen {
		private readonly IWorldAccessor worldAccessor;
		private readonly LevelManager levelManager;
		
		public GamePausedScreen(IWorldAccessor worldAccessor, LevelManager levelManager) 
			: base(AssetManager.Resolve<VisualTreeAsset>(new AssetKey("GamePausedScreen"))) {
			this.worldAccessor = worldAccessor;
			this.levelManager = levelManager;
		}

		public override bool ReturnWithEscape => false;

		protected override void OnBind(VisualElement root) {
			root.Q<Button>("Resume").clicked += this.levelManager.UnpauseGame;
			root.Q<Button>("Quit").clicked += this.worldAccessor.QuitActiveWorld;
		}
	}
}
