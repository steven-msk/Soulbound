using System;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.Debug.Metrics.View {
	public sealed class MetricsHUD {
		private readonly DebugMetricsService metricsService;
		private readonly AverageFrameCounter fpsCounter = new(10);
		private readonly AverageFrameCounter frameTimeCounter = new(10);
		private MetricBinding[] metrics = Array.Empty<MetricBinding>();
		private VisualElement root;

		public MetricsHUD(DebugMetricsService metricsService) {
			this.metricsService = metricsService;
		}

		public bool isVisible { get; private set; }

		public void OnBind(VisualElement root) {
			this.root = root;
			this.metrics = this.CreateMetricBindings(root);
		}

		public void Show() {
			this.root.style.display = DisplayStyle.Flex;
			this.isVisible = true;
		}

		public void Hide() {
			this.root.style.display = DisplayStyle.None;
			this.isVisible = false;
		}

		private MetricBinding[] CreateMetricBindings(VisualElement root) {
			return new[] {
				new LabelMetricBinding(root, "FPS", data => {
					this.fpsCounter.Tick(Read(data, DebugMetricId.Fps));
					return $"FPS: {this.fpsCounter.GetAverage():F1}";
				}),
				new LabelMetricBinding(root, "FrameTime", data => {
					this.frameTimeCounter.Tick(Read(data, DebugMetricId.FrameTime));
					return $"Frame time: {this.frameTimeCounter.GetAverage():F1}ms";
				}),
				new LabelMetricBinding(root, "FixedUpdateTime",
					data => $"Fixed update time: {Read(data, DebugMetricId.FixedUpdateTime):F1}ms"
				),
				new LabelMetricBinding(root, "TotalMemory",
					data => $"Total memory: {Read(data, DebugMetricId.TotalManagedMemory):F1}MB / {Read(data, DebugMetricId.MonoHeap):F1}MB"
				),
				new LabelMetricBinding(root, "GPUMemory",
					data => $"GPU memory: {Read(data, DebugMetricId.GpuManagedMemory):F1}MB / {Read(data, DebugMetricId.GpuReservedMemory):F1}MB"
				),
				new LabelMetricBinding(root, "GCAlloc",
					data => $"GC alloc: {Read(data, DebugMetricId.GcAlloc):F0}B"
				)
			};
		}

		public void Refresh() {
			if (!this.isVisible) return;
			DebugMetricsSnapshot data = this.metricsService.CaptureData();

			for (int i = 0; i < this.metrics.Length; i++) {
				this.metrics[i].Refresh(data);
			}
		}

		private static float Read(DebugMetricsSnapshot data, DebugMetricId id) {
			return data.TryGet(id, out float value) ? value : 0f;
		}
	}
}
