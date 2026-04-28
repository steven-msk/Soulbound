using SoulboundEngine.Client.UI.Containers;
using SoulboundEngine.Client.UI.Layouts;
using SoulboundEngine.Client.World;
using System.Collections.Generic;
using System.Linq;

namespace SoulboundEngine.Client.UI.Screens {
	public sealed class WorldListScreen : Screen {
		public const int MAX_WORLDS = 10;
		private readonly IWorldAccessor worldAccessor;

		public WorldListScreen(IWorldAccessor worldAccessor) {
			this.worldAccessor = worldAccessor;
		}

		protected override void OnBuild(IScreenObject screenObject) {
			IUIElementContainer parentContainer = GUI.Container(
				GUI.Frame.StretchTop(),
				GUI.Layout.Vertical()
					.ChildSizing(ChildSizingMode.Preferred)
					.Padding(new UnityEngine.RectOffset(0, 0, 20, 20))
			).Build(screenObject);

			IUIElementContainer savesContainer = GUI.Container(
				GUI.Frame.Stretch(),
				GUI.Layout.Horizontal()
					.ControlChildSize(true)
					.Spacing(10)
			).Build(parentContainer);
			IUIElementContainer savesCol = GUI.Container(
				GUI.Frame.Stretch(),
				GUI.Layout.Vertical()
					.ControlChildSize(true)
					.Spacing(10)
			).Build(savesContainer);
			IUIElementContainer deleteCol = GUI.Container(
				GUI.Frame.Stretch(),
				GUI.Layout.Vertical()
					.ControlChildSize(true)
					.Spacing(10)
			).Build(savesContainer);

			List<string> saves = this.worldAccessor.ListWorldSaves().ToList();
			foreach (var save in saves) {
				GUI.Button.Label()
					.Text(save)
					.OnClick(() => this.worldAccessor.EnterWorld(save))
					.Build(savesCol);

				GUI.Button.Label()
					.Text("Delete")
					.OnClick(() => { 
						this.worldAccessor.DeleteWorld(save);
						this.screenNavigator.IssueRebuild(this);
					})
					.Build(deleteCol);
			}

			IUIElementContainer newWorldContainer = GUI.Container(
				GUI.Frame.Stretch(),
				GUI.Layout.Horizontal()
					.ControlChildSize(true)
					.Align(UIAlignment.Center)
					.Spacing(10)
					.Padding(new UnityEngine.RectOffset(0, 0, 40, 40))
			).Build(GUI.Container(
					GUI.Frame.Stretch(),
					GUI.Layout.Vertical()
						.ChildSizing(ChildSizingMode.Preferred)
						.Align(UIAlignment.End)
				).Build(GUI.Container(
						GUI.Frame.Stretch(),
						GUI.Layout.Vertical()
							.ControlChildSize(true)
							.ChildForceExpandHeight(true)
							.Align(UIAlignment.End)
					).Build(parentContainer)));

			InputFieldHandle inputField = GUI.InputField.Build(newWorldContainer);

			GUI.Button.Label()
				.Text("Create new world")
				.OnClick(() => {
					string name = inputField.GetText();
					if (!string.IsNullOrEmpty(name) && this.worldAccessor.ListWorldSaves().Count() < MAX_WORLDS) {
						this.worldAccessor.CreateNewWorld(name);
						this.screenNavigator.IssueRebuild(this);
						inputField.Clear();
					}
				}).Build(newWorldContainer);

		}
	}
}
