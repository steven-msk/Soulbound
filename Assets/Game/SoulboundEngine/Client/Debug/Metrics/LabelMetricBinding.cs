using SoulboundEngine.Client.Debug.Metrics.View;
using SoulboundEngine.Client.UI.UXMLBindings;
using SoulboundEngine.Core.Registry;
using System;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.Debug.Metrics {
	public class LabelMetricBinding : MetricBinding {
		protected readonly Label output;

		public LabelMetricBinding(Label output, Func<DebugMetricsSnapshot, string> format) 
			: base(output, format) {
			this.output = output;
		}

		public LabelMetricBinding(VisualElement root, Identifier labelId, Func<DebugMetricsSnapshot, string> format) 
			: this(root.Get<Label>(labelId), format) {
		}

		public override void Refresh(DebugMetricsSnapshot data) {
			this.output.text = this.format(data);
		}
	}
}
