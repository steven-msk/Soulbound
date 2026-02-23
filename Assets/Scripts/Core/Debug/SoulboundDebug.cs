using SoulboundBackend.Core.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Core.Debug {
	public class SoulboundDebug {
		private readonly DebugConsole console;

		public SoulboundDebug(ILogger logger) {
#if !UNITY_EDITOR
			Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
			Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
			Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.ScriptOnly);
#endif
			new Logging.Logger(logger);
			this.console = new DebugConsole();
		}

		public DebugConsole GetConsole() => console;
	}
}
