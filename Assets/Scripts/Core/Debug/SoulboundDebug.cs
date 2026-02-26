using Assets.Scripts.Core.Debug;
using SoulboundBackend.Client.Input;
using SoulboundBackend.Core.Debug.Commands;
using SoulboundBackend.Core.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Logger = SoulboundBackend.Core.Debug.Logging.Logger;

namespace SoulboundBackend.Core.Debug {
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
			Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.ScriptOnly);
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
