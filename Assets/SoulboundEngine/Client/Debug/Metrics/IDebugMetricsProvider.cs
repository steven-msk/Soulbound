using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.Debug.Metrics {
	public interface IDebugMetricsProvider {
		DebugMetricsSnapshot GetData();
	}
}
