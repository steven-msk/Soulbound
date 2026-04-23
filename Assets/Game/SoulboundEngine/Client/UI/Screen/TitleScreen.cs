using SoulboundEngine.Client.UI.Containers;
using SoulboundEngine.Client.UI.Screens;
using SoulboundEngine.Common;
using SoulboundEngine.Core;
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

			foreach (var world in Soulbound.instance.ListWorldSaves()) {
				GUI.Button.Label()
					.Text(world)
					.OnClick(() => Soulbound.instance.EnterWorld(world))
					.Build(container);
			}

			GUI.Button.Label()
				.Text("new world")
				.OnClick(() => {
					string world = $"world_{Guid.NewGuid()}";
					Soulbound.instance.CreateNewWorld(world);
					Soulbound.instance.EnterWorld(world);
				})
				.Build(container);
		}
	}
}
