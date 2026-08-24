namespace SoulboundEngine.UnityClient.Debug.Metrics {
	using SoulboundEngine.Registry;
	using SoulboundEngine.UnityClient.Debug.Metrics.View;
	using SoulboundEngine.UnityClient.UI.UXMLBindings;
	using System;
	using UnityEngine.UIElements;

	public class LabelMetricBinding : MetricBinding {
		protected readonly Label output;
		private readonly Func<DebugMetricsSnapshot, string, string> formatter;
		private readonly string format;

		public LabelMetricBinding(Label output, Func<DebugMetricsSnapshot, string, string> formatter) 
			: base(output) {
			this.output = output;
			this.formatter = formatter;
			this.format = output.text;
		}

		public LabelMetricBinding(VisualElement root, Identifier labelId, Func<DebugMetricsSnapshot, string, string> formatter) 
			: this(root.Get<Label>(labelId), formatter) {
		}

		public override void Refresh(DebugMetricsSnapshot data) {
			this.output.text = this.formatter(data, this.format);
		}
	}
}
