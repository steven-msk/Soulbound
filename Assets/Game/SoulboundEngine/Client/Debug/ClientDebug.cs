using SoulboundEngine.Client.Debug.Logging.Console;
using SoulboundEngine.Client.Debug.Metrics.View;
using SoulboundEngine.Client.Input;
using UnityEngine;

namespace SoulboundEngine.Client.Debug {
	public sealed class ClientDebug {
		private readonly DebugConsole console;
		private readonly CommandLine commandLine;
		private readonly MetricsHUD metricsHud;
		private readonly IInputManager inputManager;

		public ClientDebug(ILogger logger, DebugConsole console, CommandLine commandLine, MetricsHUD metricsHud, IInputManager inputManager) {
			new Logging.Logger(logger);
			this.console = console;
			this.commandLine = commandLine;
			this.metricsHud = metricsHud;
			this.inputManager = inputManager;
#if !UNITY_EDITOR
			Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
			Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
			Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.None);
#endif
		}

		public void ToggleMetricsHUD() {
			if (!this.console.IsVisible()) {
				this.metricsHud.Toggle();
			}
		}

		public void ShowCommandLine() {
			if (!this.commandLine.IsVisible() && !this.console.IsVisible()) {
				this.commandLine.Show();
				this.inputManager.AddHandler(this.commandLine);
			}
		}

		public bool IsCommandLineVisible() {
			return this.commandLine.IsVisible();
		}

		public void HideCommandLine() {
			if (this.commandLine.IsVisible()) {
				this.commandLine.Toggle();
				this.inputManager.RemoveHandler(this.commandLine);
			}
		}

		public void ToggleConsole() {
			this.HideCommandLine();
			if (this.metricsHud.IsVisible()) {
				this.metricsHud.Toggle();
			}
			this.console.Toggle();
		}
	}
}
