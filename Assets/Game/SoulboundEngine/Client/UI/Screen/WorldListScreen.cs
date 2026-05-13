using SoulboundEngine.Client.World;

namespace SoulboundEngine.Client.UI.Screen {
	public sealed class WorldListScreen : Screen {
		public const int MAX_WORLDS = 10;
		private readonly IWorldAccessor worldAccessor;

		public WorldListScreen(IWorldAccessor worldAccessor) {
			this.worldAccessor = worldAccessor;
		}

		protected override void OnBuild(IScreenHandle handle) {
			//IUIElementContainer parentContainer = GUI.Container(
			//	GUI.Frame.StretchTop(),
			//	GUI.Layout.Vertical()
			//		.ChildSizing(ChildSizingMode.Preferred)
			//		.Padding(new UnityEngine.RectOffset(0, 0, 20, 20))
			//).Build(screenObject);

			//IUIElementContainer savesContainer = GUI.Container(
			//	GUI.Frame.Stretch(),
			//	GUI.Layout.Horizontal()
			//		.ControlChildSize(true)
			//		.Spacing(10)
			//).Build(parentContainer);
			//IUIElementContainer savesCol = GUI.Container(
			//	GUI.Frame.Stretch(),
			//	GUI.Layout.Vertical()
			//		.ControlChildSize(true)
			//		.Spacing(10)
			//).Build(savesContainer);
			//IUIElementContainer deleteCol = GUI.Container(
			//	GUI.Frame.Stretch(),
			//	GUI.Layout.Vertical()
			//		.ControlChildSize(true)
			//		.Spacing(10)
			//).Build(savesContainer);

			//List<WorldSave> saves = this.worldAccessor.ListWorldSaves().ToList();
			//foreach (var save in saves) {
			//	GUI.Button.Label()
			//		.Text($"{save.name} ({save.seed})")
			//		.OnClick(() => this.worldAccessor.EnterWorld(save.name))
			//		.Build(savesCol);

			//	GUI.Button.Label()
			//		.Text("Delete")
			//		.OnClick(() => { 
			//			this.worldAccessor.DeleteWorld(save.name);
			//			this.screenManager.IssueRebuild(this);
			//		})
			//		.Build(deleteCol);
			//}

			//IUIElementContainer newWorldContainer = GUI.Container(
			//	GUI.Frame.Stretch(),
			//	GUI.Layout.Horizontal()
			//		.ControlChildSize(true)
			//		.Align(UIAlignment.Center)
			//		.Spacing(10)
			//		.Padding(new UnityEngine.RectOffset(0, 0, 40, 40))
			//).Build(GUI.Container(
			//		GUI.Frame.Stretch(),
			//		GUI.Layout.Vertical()
			//			.ChildSizing(ChildSizingMode.Preferred)
			//			.Align(UIAlignment.End)
			//	).Build(GUI.Container(
			//			GUI.Frame.Stretch(),
			//			GUI.Layout.Vertical()
			//				.ControlChildSize(true)
			//				.ChildForceExpandHeight(true)
			//				.Align(UIAlignment.End)
			//		).Build(parentContainer)));

			//IUIElementContainer dataContainer = GUI.Container(
			//	GUI.Frame.Stretch(),
			//	GUI.Layout.Vertical()
			//		.ChildSizing(ChildSizingMode.Preferred)
			//).Build(newWorldContainer);

			//InputFieldHandle nameField = GUI.InputField
			//	.Placeholder("World name...")
			//	.Build(dataContainer);
			//InputFieldHandle seedField = GUI.InputField
			//	.Placeholder("Seed... (leave empty for random)")
			//	.Build(dataContainer);

			//GUI.Button.Label()
			//	.Text("Create new world")
			//	.OnClick(() => {
			//		string name = nameField.GetText();
			//		if (!string.IsNullOrEmpty(name) && this.worldAccessor.ListWorldSaves().Count() < MAX_WORLDS) {
			//			int seed = WorldManager.GetRandomSeed();
			//			if (!string.IsNullOrEmpty(seedField.GetText())) {
			//				if (!int.TryParse(seedField.GetText(), out seed)) {
			//					Logger.LogError("Invalid seed: {}", seedField.GetText());
			//					return;
			//				}
			//			}
			//			this.worldAccessor.CreateNewWorld(name, seed);
			//			this.screenManager.IssueRebuild(this);
			//			nameField.Clear();
			//			seedField.Clear();
			//		}
			//	}).Build(newWorldContainer);

		}

	}
}
