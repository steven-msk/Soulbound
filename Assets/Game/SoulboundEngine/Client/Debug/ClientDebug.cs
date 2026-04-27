using SoulboundEngine.Client.Debug.Logging.Console;
using SoulboundEngine.Client.Debug.Metrics.View;
using SoulboundEngine.Client.Input;
using System;
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

		public void ShowCommandLine(Action onHide) {
			if (!this.commandLine.IsVisible()) {
				this.commandLine.Show();
				this.commandLine.AddHideAction(onHide);
				this.commandLine.AddHideAction(() => this.inputManager.RemoveHandler(this.commandLine));
				this.inputManager.AddHandler(this.commandLine);
			}
		}

		public void ToggleConsole() {
			if (this.metricsHud.IsVisible()) {
				this.metricsHud.Toggle();
			}
			this.console.Toggle();
		}

		public bool IsConsoleVisible() {
			return this.console.IsVisible();
		}

		public bool IsMetricsHUDVisible() {
			return this.metricsHud.IsVisible();
		}
	}
}
