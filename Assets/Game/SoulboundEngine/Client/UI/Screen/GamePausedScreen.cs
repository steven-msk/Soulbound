using SoulboundEngine.Client.World;
using SoulboundEngine.Client.World.Level;
using SoulboundEngine.Common;
using SoulboundEngine.Core.Assets;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI.Screen {
	[PROTOTYPICAL]
	public sealed class GamePausedScreen : UxmlScreen {
		private const string RESUME_ELEMENT = "Resume";
		private const string QUIT_ELEMENT = "Quit";
		private readonly IWorldAccessor worldAccessor;
		private readonly LevelManager levelManager;
		
		public GamePausedScreen(IWorldAccessor worldAccessor, LevelManager levelManager) 
			: base(AssetManager.Resolve<VisualTreeAsset>(new AssetKey("GamePausedScreen"))) {
			this.worldAccessor = worldAccessor;
			this.levelManager = levelManager;
		}

		public override bool IsOpaque => false;
		public override bool CloseOnEsc => false;

		protected override void OnBind(VisualElement root) {
			root.Q<Button>(RESUME_ELEMENT).clicked += this.levelManager.UnpauseGame;
			root.Q<Button>(QUIT_ELEMENT).clicked += this.worldAccessor.QuitActiveWorld;
		}
	}
}
