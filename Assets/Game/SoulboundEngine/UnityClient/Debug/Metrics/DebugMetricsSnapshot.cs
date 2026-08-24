namespace SoulboundEngine.UnityClient.Debug.Metrics {
	using System.Collections.Generic;

	public readonly struct DebugMetricsSnapshot {
		private readonly Dictionary<DebugMetricId, object> data;

		public DebugMetricsSnapshot(DebugMetricData[] buffer, int count) {
			this.data = new Dictionary<DebugMetricId, object>(count);
			for (int i = 0; i < count; i++) {
				this.data[buffer[i].id] = buffer[i].value;
			}
		}

		public readonly object Get(DebugMetricId id) => this.data[id];

		public readonly bool TryGet(DebugMetricId id, out object value) {
			return this.data.TryGetValue(id, out value);
		}
	}
}
