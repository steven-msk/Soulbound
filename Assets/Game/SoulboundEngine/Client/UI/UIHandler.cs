using SoulboundEngine.Client.Debug;
using SoulboundEngine.Client.Debug.Logging.Console;
using SoulboundEngine.Client.Debug.Metrics.View;
using SoulboundEngine.Client.UI.Screen;
using UnityEngine.UIElements;

#nullable enable

namespace SoulboundEngine.Client.UI {
	public sealed class UIHandler {
		private ScreenManager screenManager = null!;
		private UXMLScreenRoot screenRoot = null!;
		private readonly CommandLine commandLine;
		private readonly LogConsole logConsole;
		private readonly MetricsHUD metricsHud;

		public UIHandler(CommandLine commandLine, LogConsole logConsole, MetricsHUD metricsHud) {
			this.commandLine = commandLine;
			this.logConsole = logConsole;
			this.metricsHud = metricsHud;
		}

		public void SetUIDocument(UIDocument uiDocument) {
			if (this.screenRoot != null) {
				this.commandLine.Dispose();
				this.logConsole.Dispose();
				this.metricsHud.Dispose();
				this.screenManager.Flush();
			}
			this.screenRoot = new UXMLScreenRoot(uiDocument);
			this.screenManager = new ScreenManager(this.screenRoot);

			VisualElement commandLineElement = this.screenManager.CreateScreenRoot();
			CommandLine.CreateRoot(commandLineElement);
			this.screenRoot.Attach(commandLineElement);
			this.commandLine.OnBind(commandLineElement);

			VisualElement logConsoleElement = this.screenManager.CreateScreenRoot();
			LogConsole.CreateRoot(logConsoleElement);
			this.screenRoot.Attach(logConsoleElement);
			this.logConsole.OnBind(logConsoleElement);

			VisualElement metricsHudElement = this.screenManager.CreateScreenRoot();
			MetricsHUD.CreateRoot(metricsHudElement);
			this.screenRoot.Attach(metricsHudElement);
			this.metricsHud.OnBind(metricsHudElement);
		}

		public IScreenHandle PushScreen(Screen.Screen screen) => this.screenManager.PushScreen(screen);
		public void PopScreen(IScreenHandle handle) => this.screenManager.PopScreen(handle);

		public bool HasKeyboardFocus() => this.screenManager.HasKeyboardFocus();
		public bool IsPointerOverUI() => this.screenManager.IsPointerOverUI();

		public void PushInputFocus(IInputFocusable focus) => this.screenManager.PushInputFocus(focus);
		public void PopInputFocus(IInputFocusable focus) => this.screenManager.PopInputFocus(focus);

		public void FlushScreens() => this.screenManager.Flush();
	}
}
