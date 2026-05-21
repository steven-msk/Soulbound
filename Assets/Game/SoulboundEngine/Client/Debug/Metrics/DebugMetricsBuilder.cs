namespace SoulboundEngine.Client.Debug.Metrics {
	public ref struct DebugMetricsBuilder {
		private readonly DebugMetricData[] buffer;
		private int count;

		public DebugMetricsBuilder(DebugMetricData[] buffer) {
			this.buffer = buffer;
			count = 0;
		}

		public readonly DebugMetricsSnapshot Build() {
			return new DebugMetricsSnapshot(buffer, count);
		}

		public void Add(DebugMetricId id, int value) {
			buffer[count++] = new DebugMetricData {
				id = id,
				value = value
			};
		}

		public void Add(DebugMetricId id, float value) {
			buffer[count++] = new DebugMetricData {
				id = id,
				value = value
			};
		}
	}
}
