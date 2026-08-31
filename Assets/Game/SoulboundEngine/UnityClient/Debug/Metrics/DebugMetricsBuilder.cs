namespace SoulboundEngine.UnityClient.Debug.Metrics {
	public ref struct DebugMetricsBuilder {
		private readonly DebugMetricData[] buffer;
		private int count;

		public DebugMetricsBuilder(DebugMetricData[] buffer) {
			this.buffer = buffer;
			this.count = 0;
		}

		public readonly DebugMetricsSnapshot Build() {
			return new DebugMetricsSnapshot(this.buffer, this.count);
		}

		public void Add(DebugMetricId id, int value) {
			this.Add(id, (object)value);
		}

		public void Add(DebugMetricId id, float value) {
			this.Add(id, (object)value);
		}

		public void Add(DebugMetricId id, object value) {
			this.buffer[this.count++] = new DebugMetricData {
				id = id,
				value = value
			};
		}
	}
}
