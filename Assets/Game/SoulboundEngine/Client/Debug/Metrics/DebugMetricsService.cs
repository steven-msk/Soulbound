using System.Buffers;
using System.Collections.Generic;

namespace SoulboundEngine.Client.Debug.Metrics {
	public sealed class DebugMetricsService {
		private readonly List<IDebugMetricsSource> sources = new();

		public DebugMetricsSnapshot CaptureData() {
			DebugMetricData[] buffer = ArrayPool<DebugMetricData>.Shared.Rent(32);
			DebugMetricsBuilder builder = new(buffer);

			try {
				for (int i = 0; i < sources.Count; i++) {
					sources[i].CollectDebugData(ref builder);
				}

				return builder.Build();
			} finally {
				ArrayPool<DebugMetricData>.Shared.Return(buffer);
			}
		}

		public void RegisterSource(IDebugMetricsSource source) => sources.Add(source);
		public void UnregisterSource(IDebugMetricsSource source) => sources.Remove(source);
	}
}
