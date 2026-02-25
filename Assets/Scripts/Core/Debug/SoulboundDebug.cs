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

namespace SoulboundBackend.Core.Debug {
	public sealed class SoulboundDebug : IInputContext {
		private readonly DebugConsole console;
		private readonly CommandLine commandLine;
		private readonly MetricsHUD metricsHud;
		private readonly DebugMetricsService metricsService;

		public SoulboundDebug(ILogger logger, DebugMetricsService metricsService, CommandProcessor commandProcessor) {
			new Logging.Logger(logger);
			console = new DebugConsole();
			commandLine = new CommandLine(commandProcessor);
			this.metricsService = metricsService;
			metricsHud = new MetricsHUD(metricsService);
#if !UNITY_EDITOR
			Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
			Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
			Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.ScriptOnly);
#endif
		}

		public DebugConsole GetConsole() => console;

		bool IInputContext.HandleInput(in InputEvent inputEvent) {
			if (inputEvent.token.Equals(InputTokens.Debug.toggleConsole)
					|| inputEvent.token.Equals(InputTokens.Keyboard.ESC)
					&& inputEvent.phase == UnityEngine.InputSystem.InputActionPhase.Performed) {
				console.Toggle();
				return true;
			}
			if (inputEvent.token.Equals(InputTokens.Debug.enterCommand)) {
				commandLine.Show();
				return true;
			}

			if (inputEvent.token.Equals(InputTokens.Debug.toggleMetrics)) {
				metricsHud.Toggle();
				return true;
			}

			return false;
		}

		public DebugMetricsService GetMetricsService() => metricsService;
	}
}
