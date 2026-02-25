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
		private readonly MetricsHUD metricsHud;
		private readonly DebugMetricsService metricsService;
		private readonly CommandProcessor commandProcessor;

		public SoulboundDebug(ILogger logger, DebugMetricsService metricsService, CommandProcessor commandProcessor) {
#if !UNITY_EDITOR
			Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
			Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
			Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.ScriptOnly);
#endif
			new Logging.Logger(logger);
			this.commandProcessor = commandProcessor;
			console = new DebugConsole(commandProcessor);
			this.metricsService = metricsService;
			metricsHud = new MetricsHUD(metricsService);
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
				if (!console.IsVisible()) console.Toggle();
				console.StartCommandInput();
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
