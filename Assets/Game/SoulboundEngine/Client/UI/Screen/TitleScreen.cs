using SoulboundEngine.Client.UI.Containers;
using SoulboundEngine.Client.UI.Layouts;
using SoulboundEngine.Client.World;
using SoulboundEngine.Common;
using SoulboundEngine.Core;

namespace SoulboundEngine.Client.UI.Screens {
	public class TitleScreen : Screen {
		private readonly IWorldAccessor worldAccessor;

		public TitleScreen(IWorldAccessor worldAccessor) {
			this.worldAccessor = worldAccessor;
			this.supportsEscapePop = false;
		}

		[PROTOTYPICAL]
		protected override void OnBuild(IScreenObject screenObject) {
			IUIElementContainer container = GUI.Container(
				GUI.Frame.Stretch(),
				GUI.Layout.Vertical()
					.Align(UIAlignment.Center)
					.ControlChildWidth(true)
			).Build(screenObject);

			GUI.Button.Label()
				.Text("Play")
				.Size(26f)
				.OnClick(() => this.screenNavigator.PushScreen(new WorldListScreen(this.worldAccessor)))
				.Build(container);

			GUI.Button.Label()
				.Text("Exit")
				.Size(26f)
				.OnClick(() => Soulbound.Instance.CloseGame())
				.Build(container);
		}
	}
}
