using SoulboundEngine.Client.Debug.Logging.Console;
using SoulboundEngine.Client.Debug.Metrics.View;
using SoulboundEngine.Client.Input;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SoulboundEngine.Client.Debug {
	public sealed class ClientDebug : IInputEventHandler {
		int IInputEventHandler.priority => 5005;
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

		IEnumerable<InputEventListener> IInputEventHandler.GetListeners() {
			InputEventListener GetCommandLineListener(InputToken token, Key key) {
				return InputEventListener.ConsumePerformed(token, _ => {
					this.commandLine.HandleKey(key);
				});
			}

			return new InputEventListener[] {
				InputEventListener.ConsumePerformed(InputTokens.Debug.enterCommand, _ => {
					bool alreadyVisible = this.commandLine.IsVisible();
					if (!alreadyVisible) this.commandLine.Show();
				}),

				GetCommandLineListener(InputTokens.Keyboard.TAB, Key.Tab),
				GetCommandLineListener(InputTokens.Keyboard.ARROW_UP, Key.UpArrow),
				GetCommandLineListener(InputTokens.Keyboard.ARROW_DOWN, Key.DownArrow),
				GetCommandLineListener(InputTokens.Keyboard.ESC, Key.Escape),
				GetCommandLineListener(InputTokens.Keyboard.BACKSPACE, Key.Backspace),

				InputEventListener.ConsumePerformed(InputTokens.Debug.toggleConsole, _ => this.console.Toggle())
			};
		}

		public void ToggleMetricsHUD() {
			this.metricsHud.Toggle();
		}
	}
}
