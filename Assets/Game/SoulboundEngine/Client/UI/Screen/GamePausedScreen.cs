using SoulboundEngine.Client.UI.UXMLBindings;
using SoulboundEngine.Client.World;
using SoulboundEngine.Client.World.Level;
using SoulboundEngine.Common;
using SoulboundEngine.Core.Assets;
using SoulboundEngine.Core.Registry;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI.Screen {
	[PROTOTYPICAL]
	public sealed class GamePausedScreen : UxmlScreen {
		private static readonly Identifier RESUME_ELEMENT = Identifier.Of("soulbound:game_paused_screen/resume");
		private static readonly Identifier QUIT_ELEMENT = Identifier.Of("soulbound:game_paused_screen/quit");
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
			root.Get<Button>(RESUME_ELEMENT).clicked += this.levelManager.UnpauseGame;
			root.Get<Button>(QUIT_ELEMENT).clicked += this.worldAccessor.QuitActiveWorld;
		}
	}
}
