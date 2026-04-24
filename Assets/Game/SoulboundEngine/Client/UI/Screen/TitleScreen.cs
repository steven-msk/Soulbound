using SoulboundEngine.Client.UI.Containers;
using SoulboundEngine.Common;
using System;

namespace SoulboundEngine.Client.UI.Screens {
	public class TitleScreen : Screen {

		[PROTOTYPICAL]
		protected override void OnBuild(IScreenObject screenObject) {
			IUIElementContainer container = GUI.Container(
				GUI.Frame.StretchWithPadding(100f),
				GUI.Layout.Vertical()
					.ChildForceExpandWidth(true)
					.ControlChildWidth(true)
			).Build(screenObject);

			foreach (var world in SoulboundClient.Instance.ListWorldSaves()) {
				GUI.Button.Label()
					.Text(world)
					.OnClick(() => SoulboundClient.Instance.EnterWorld(world))
					.Build(container);
			}

			GUI.Button.Label()
				.Text("new world")
				.OnClick(() => {
					string world = $"world_{Guid.NewGuid()}";
					SoulboundClient.Instance.CreateNewWorld(world);
					SoulboundClient.Instance.EnterWorld(world);
				})
				.Build(container);
		}
	}
}
