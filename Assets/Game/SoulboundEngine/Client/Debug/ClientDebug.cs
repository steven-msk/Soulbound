using SoulboundEngine.Client.Debug.Logging.Console;
using SoulboundEngine.Client.Debug.Metrics.View;
using SoulboundEngine.Client.Input;
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

		bool IInputContext.HandleInput(in InputEvent inputEvent) {
			if (!commandLine.IsVisible() && inputEvent.Performed(InputTokens.Debug.enterCommand)) {
				commandLine.Show();
				return true;
			}

			// this is risky because ALL inputs are consumed
			// but it works for now
			if (commandLine.IsVisible()) {
				if (inputEvent.Performed(InputTokens.Keyboard.TAB)) commandLine.HandleKey(Key.Tab);
				if (inputEvent.Performed(InputTokens.Keyboard.ARROW_UP)) commandLine.HandleKey(Key.UpArrow);
				if (inputEvent.Performed(InputTokens.Keyboard.ARROW_DOWN)) commandLine.HandleKey(Key.DownArrow);
				if (inputEvent.Performed(InputTokens.Keyboard.ESC)) commandLine.HandleKey(Key.Escape);
				if (inputEvent.Performed(InputTokens.Keyboard.BACKSPACE)) commandLine.HandleKey(Key.Backspace);
				return true;
			}

			if (inputEvent.Performed(InputTokens.Debug.toggleConsole)) {
				console.Toggle();
				return true;
			}

			if (inputEvent.Performed(InputTokens.Debug.toggleMetrics)) {
				metricsHud.Toggle();
				return true;
			}

			return false;
		}
	}
}
