using SoulboundEngine.Client.UI.Screen;
using UnityEngine.UIElements;

#nullable enable

namespace SoulboundEngine.Client.UI {
	public sealed class UIHandler {
		private ScreenManager screenManager;
		private UXMLScreenRoot screenRoot;

		public UIHandler(UIDocument uiDocument) {
			this.screenRoot = new UXMLScreenRoot(uiDocument);
			this.screenManager = new ScreenManager(this.screenRoot);
		}

		public void SetUIDocument(UIDocument uiDocument) {
			this.screenRoot = new UXMLScreenRoot(uiDocument);
			this.screenManager.Flush();
			this.screenManager = new ScreenManager(this.screenRoot);
		}

		public IScreenHandle PushScreen(Screen.Screen screen) => this.screenManager.PushScreen(screen);
		public void PopScreen(IScreenHandle handle) => this.screenManager.PopScreen(handle);

		public void FlushScreens() => this.screenManager.Flush();

		public IScreenNavigator GetScreenNavigator() => this.screenManager;
	}
}
