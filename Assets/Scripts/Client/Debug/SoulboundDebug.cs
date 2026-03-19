using SoulboundBackend.Client.Debug.Commands;
using SoulboundBackend.Client.Debug.Logging.Console;
using SoulboundBackend.Client.Debug.Metrics;
using SoulboundBackend.Client.Debug.Metrics.View;
using SoulboundBackend.Client.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using Logger = SoulboundBackend.Client.Debug.Logging.Logger;

namespace SoulboundBackend.Client.Debug {
	public sealed class SoulboundDebug : IInputContext {
		int IInputContext.priority => int.MaxValue;
		private readonly DebugConsole console;
		private readonly CommandLine commandLine;
		private readonly MetricsHUD metricsHud;

		public SoulboundDebug(ILogger logger, DebugMetricsService metricsService, CommandProcessor commandProcessor) {
			new Logging.Logger(logger);
			console = new DebugConsole();
			commandLine = new CommandLine(commandProcessor);
			metricsHud = new MetricsHUD(metricsService);
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
