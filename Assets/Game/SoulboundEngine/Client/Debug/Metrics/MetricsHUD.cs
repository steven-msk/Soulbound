using SoulboundEngine.Client.UI;
using SoulboundEngine.Core.Registry;
using System;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.Debug.Metrics.View {
	public sealed class MetricsHUD : UXMLWidget {
		private static readonly Identifier FPS_ELEMENT = Identifier.Of("soulbound:metrics_hud/fps");
		private static readonly Identifier FRAME_TIME_ELEMENT = Identifier.Of("soulbound:metrics_hud/frame_time");
		private static readonly Identifier FIXED_UPDATE_TIME_ELEMENT = Identifier.Of("soulbound:metrics_hud/fixed_update_time");
		private static readonly Identifier TOTAL_MEMORY_ELEMENT = Identifier.Of("soulbound:metrics_hud/total_memory");
		private static readonly Identifier GPU_MEMORY_ELEMENT = Identifier.Of("soulbound:metrics_hud/gpu_memory");
		private static readonly Identifier GC_ALLOC_ELEMENT = Identifier.Of("soulbound:metrics_hud/gc_alloc");
		private readonly DebugMetricsService metricsService;
		private readonly AverageFrameCounter fpsCounter = new(10);
		private readonly AverageFrameCounter frameTimeCounter = new(10);
		private MetricBinding[] metrics = Array.Empty<MetricBinding>();

		public MetricsHUD(DebugMetricsService metricsService) {
			this.metricsService = metricsService;
		}

		public override void OnBind(VisualElement root) {
			base.OnBind(root);
			this.metrics = this.CreateMetricBindings(root);
		}

		private MetricBinding[] CreateMetricBindings(VisualElement root) {
			return new[] {
				new LabelMetricBinding(root, FPS_ELEMENT, data => {
					this.fpsCounter.Tick(Read(data, DebugMetricId.Fps));
					return $"FPS: {this.fpsCounter.GetAverage():F1}";
				}),
				new LabelMetricBinding(root, FRAME_TIME_ELEMENT, data => {
					this.frameTimeCounter.Tick(Read(data, DebugMetricId.FrameTime));
					return $"Frame time: {this.frameTimeCounter.GetAverage():F1}ms";
				}),
				new LabelMetricBinding(root, FIXED_UPDATE_TIME_ELEMENT,
					data => $"Fixed update time: {Read(data, DebugMetricId.FixedUpdateTime):F1}ms"
				),
				new LabelMetricBinding(root, TOTAL_MEMORY_ELEMENT,
					data => $"Total memory: {Read(data, DebugMetricId.TotalManagedMemory):F1}MB / {Read(data, DebugMetricId.MonoHeap):F1}MB"
				),
				new LabelMetricBinding(root, GPU_MEMORY_ELEMENT,
					data => $"GPU memory: {Read(data, DebugMetricId.GpuManagedMemory):F1}MB / {Read(data, DebugMetricId.GpuReservedMemory):F1}MB"
				),
				new LabelMetricBinding(root, GC_ALLOC_ELEMENT,
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
