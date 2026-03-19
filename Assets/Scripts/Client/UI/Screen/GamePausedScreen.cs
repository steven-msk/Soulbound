using SoulboundBackend.Client.UI.Containers;
using SoulboundBackend.Client.UI.Layouts;
using SoulboundBackend.Common;
using SoulboundBackend.Core;

namespace SoulboundBackend.Client.UI.Screens {
	[PROTOTYPICAL]
	public sealed class GamePausedScreen : Screen {

		protected override void OnBuild(IScreenObject screenObject) {
			IUIElementContainer container = GUI.Container(
				GUI.Frame.Stretch(),
				GUI.Layout.Vertical()
					.Align(UIAlignment.Center)
					.ControlChildSize(true)
			).Build(screenObject);

			GUI.Button.Label().Text("Resume").Build(container);
			GUI.Button.Label().Text("Settings").Build(container);

			GUI.Button.Label().Text("Quit To Title Screen")
				.OnClick(Soulbound.instance.QuitActiveWorld)
				.Build(container);
		}
	}
}
