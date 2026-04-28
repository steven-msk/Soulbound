using SoulboundEngine.Client.UI.Containers;
using SoulboundEngine.Client.UI.Layouts;
using SoulboundEngine.Client.World;
using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Common;

namespace SoulboundEngine.Client.UI.Screens {
	[PROTOTYPICAL]
	public sealed class GamePausedScreen : Screen {
		private readonly IWorldAccessor worldAccessor;
		private readonly LevelManager levelManager;
		
		public GamePausedScreen(IWorldAccessor worldAccessor, LevelManager levelManager) {
			this.worldAccessor = worldAccessor;
			this.levelManager = levelManager;
			this.supportsEscapePop = false;
		}

		protected override void OnBuild(IScreenObject screenObject) {
			IUIElementContainer container = GUI.Container(
				GUI.Frame.Stretch(),
				GUI.Layout.Vertical()
					.Align(UIAlignment.Center)
					.ControlChildSize(true)
			).Build(screenObject);

			GUI.Button.Label().Text("Resume")
				.OnClick(this.levelManager.UnpauseGame)
				.Build(container);

			GUI.Button.Label().Text("Quit To Title Screen")
				.OnClick(this.worldAccessor.QuitActiveWorld)
				.Build(container);
		}
	}
}
