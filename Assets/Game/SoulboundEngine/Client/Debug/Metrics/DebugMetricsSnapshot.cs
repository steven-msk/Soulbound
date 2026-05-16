using System.Collections.Generic;

namespace SoulboundEngine.Client.Debug.Metrics {
	public readonly struct DebugMetricsSnapshot {
		private readonly Dictionary<DebugMetricId, float> data;

		public DebugMetricsSnapshot(DebugMetricData[] buffer, int count) {
			data = new Dictionary<DebugMetricId, float>(count);
			for (int i = 0; i < count; i++) {
				data[buffer[i].id] = buffer[i].value;
			}
		}

		public readonly float Get(DebugMetricId id) => data[id];

		public readonly bool TryGet(DebugMetricId id, out float value) {
			return data.TryGetValue(id, out value);
		}
	}
}
