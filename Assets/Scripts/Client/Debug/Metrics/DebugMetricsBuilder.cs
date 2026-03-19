using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.Debug.Metrics {
	public ref struct DebugMetricsBuilder {
		private readonly DebugMetricData[] buffer;
		private int count;

		public DebugMetricsBuilder(DebugMetricData[] buffer) {
			this.buffer = buffer;
			count = 0;
		}

		public readonly DebugMetricsSnapshot Build() {
			return new DebugMetricsSnapshot(buffer[..count], count);
		}

		public void Add(string label, int value) {
			buffer[count++] = new DebugMetricData {
				valueType = typeof(int),
				label = label,
				value = value
			};
		}

		public void Add(string label, float value) {
			buffer[count++] = new DebugMetricData {
				valueType = typeof(float),
				label = label,
				value = value
			};
		}
	}
}
