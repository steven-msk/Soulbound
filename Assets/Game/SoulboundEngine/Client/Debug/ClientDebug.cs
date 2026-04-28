using SoulboundEngine.Client.Debug.Logging.Console;
using SoulboundEngine.Client.Debug.Metrics.View;
using SoulboundEngine.Client.Input;
using System;
using UnityEngine;

namespace SoulboundEngine.Client.Debug {
	[Obsolete]
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

		}

		public void ShowMetricsHUD(bool show) {
			if (!this.console.IsVisible()) {
				this.metricsHud.Toggle();
			}
		}

		public void ShowCommandLine(Action onHide) {
			if (!this.commandLine.IsVisible()) {
				this.commandLine.Show();
			}
		}

		public void ToggleConsole() {
			if (this.metricsHud.IsVisible()) {
				this.metricsHud.Toggle();
			}
			this.console.Toggle();
		}

		public bool IsCommandLineVisible() {
			return this.commandLine.IsVisible();
		}

		public bool IsConsoleVisible() {
			return this.console.IsVisible();
		}

		public bool IsMetricsHUDVisible() {
			return this.metricsHud.IsVisible();
		}
	}
}
