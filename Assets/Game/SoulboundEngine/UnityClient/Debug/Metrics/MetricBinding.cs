namespace SoulboundEngine.UnityClient.Debug.Metrics.View {
	using UnityEngine.UIElements;

	public abstract class MetricBinding {
		protected readonly VisualElement root;

		protected MetricBinding(VisualElement root) {
			this.root = root;
		}

		public abstract void Refresh(DebugMetricsSnapshot data);
	}
}
