using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.Debug.Metrics {
	public readonly struct DebugMetricsSnapshot {
		private readonly Dictionary<string, object> data;

		public DebugMetricsSnapshot(DebugMetricData[] buffer, int count) {
			data = new Dictionary<string, object>(count);
			for (int i = 0; i < count; i++) {
				data[buffer[i].label] = buffer[i].value switch {
					int => (int)buffer[i].value,
					float => (float)buffer[i].value,
					_ => throw new NotSupportedException($"Debug metric type {buffer[i].GetType()} not supported")
				};
			}
		}

		public readonly object Get(string key) => data[key];
		public readonly bool TryGet(string key, out object value) {
			return data.TryGetValue(key, out value);
		}

		public readonly T Get<T>(string key) => (T)data[key];
		public readonly bool TryGet<T>(string key, out T value) {
			bool found = data.TryGetValue(key, out object boxed);
			value = (T)boxed;
			return found;
		}
	}
}
