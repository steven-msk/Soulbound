using UnityEngine;

namespace SoulboundBackend.Client.Debug.Logging.Console {
	public interface IDebugConsoleHandle {
		void AddLog(string condition, string stackTrace, LogType logType);
	}
}
