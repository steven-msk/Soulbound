using SoulboundEngine.Client.Debug.Logging.Console;
using SoulboundEngine.Client.Debug.Metrics.View;
using SoulboundEngine.Client.Input;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SoulboundEngine.Client.Debug {
	public sealed class ClientDebug : IInputContext {
		int IInputContext.priority => int.MaxValue;
		private readonly DebugConsole console;
		private readonly CommandLine commandLine;
		private readonly MetricsHUD metricsHud;

		public ClientDebug(ILogger logger, DebugConsole console, CommandLine commandLine, MetricsHUD metricsHud) {
			new Logging.Logger(logger);
			this.console = console;
			this.commandLine = commandLine;
			this.metricsHud = metricsHud;
#if !UNITY_EDITOR
			Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
			Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
			Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.None);
#endif
		}

		[Obsolete]
		bool IInputContext.HandleInput(in InputEvent inputEvent) {
			if (!this.commandLine.IsVisible() && inputEvent.Performed(InputTokens.Debug.enterCommand)) {
				this.commandLine.Show();
				return true;
			}

			// this is risky because ALL inputs are consumed
			// but it works for now
			if (this.commandLine.IsVisible()) {
				if (inputEvent.Performed(InputTokens.Keyboard.TAB)) this.commandLine.HandleKey(Key.Tab);
				if (inputEvent.Performed(InputTokens.Keyboard.ARROW_UP)) this.commandLine.HandleKey(Key.UpArrow);
				if (inputEvent.Performed(InputTokens.Keyboard.ARROW_DOWN)) this.commandLine.HandleKey(Key.DownArrow);
				if (inputEvent.Performed(InputTokens.Keyboard.ESC)) this.commandLine.HandleKey(Key.Escape);
				if (inputEvent.Performed(InputTokens.Keyboard.BACKSPACE)) this.commandLine.HandleKey(Key.Backspace);
				return true;
			}

			if (inputEvent.Performed(InputTokens.Debug.toggleConsole)) {
				this.console.Toggle();
				return true;
			}

			return false;
		}

		public void ToggleMetricsHUD() {
			this.metricsHud.Toggle();
		}
	}
}
