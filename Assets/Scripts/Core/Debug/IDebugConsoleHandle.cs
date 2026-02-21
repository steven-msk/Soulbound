using SoulboundBackend.Client.UI;
using SoulboundBackend.Core.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core.Debug {
	public interface IDebugConsoleHandle : IUIElementHandle {
		void AddLogEntry(LogEntry entry);
	}
}
