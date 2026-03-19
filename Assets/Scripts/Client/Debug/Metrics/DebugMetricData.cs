using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.Debug.Metrics {
	public struct DebugMetricData {
		public string label;
		public Type valueType;
		public object value;
	}
}
