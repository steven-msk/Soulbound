using System;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.Debug.Metrics.View {
	public abstract class MetricBinding {
		protected readonly VisualElement root;
		protected readonly Func<DebugMetricsSnapshot, string> format;

		protected MetricBinding(VisualElement root, Func<DebugMetricsSnapshot, string> format) {
			this.root = root;
			this.format = format;
		}

		public abstract void Refresh(DebugMetricsSnapshot data);
	}
}
