using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.Debug.Logging {
	[Flags]
	public enum LogLevel {
		Info		= 1 << 0,
		Warning		= 1 << 1,
		Error		= 1 << 2,
		Fatal		= 1 << 3
	}
}
