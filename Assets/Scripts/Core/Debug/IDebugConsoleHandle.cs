using SoulboundBackend.Client.UI;
using SoulboundBackend.Core.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Core.Debug {
	public interface IDebugConsoleHandle {
		void AddLog(string condition, string stackTrace, LogType logType);
		void StartCommandInput(Transform parent);
	}
}
