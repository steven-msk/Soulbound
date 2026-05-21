using SoulboundEngine.Client.Debug.Metrics.View;
using System;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.Debug.Metrics {
	public class LabelMetricBinding : MetricBinding {
		protected readonly Label output;

		public LabelMetricBinding(Label output, Func<DebugMetricsSnapshot, string> format) 
			: base(output, format) {
			this.output = output;
		}

		public LabelMetricBinding(VisualElement root, string labelName, Func<DebugMetricsSnapshot, string> format) 
			: this(root.Q<Label>(labelName), format) {
		}

		public override void Refresh(DebugMetricsSnapshot data) {
			this.output.text = this.format(data);
		}
	}
}
