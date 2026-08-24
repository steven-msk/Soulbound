namespace SoulboundEngine.UnityClient.Debug.Metrics.View {
	using SoulboundEngine.Common;
	using SoulboundEngine.Registry;
	using SoulboundEngine.UnityClient.Assets;
	using SoulboundEngine.UnityClient.Settings;
	using SoulboundEngine.UnityClient.UI;
	using System;
	using UnityEngine.UIElements;

#nullable enable

	public sealed class MetricsHUD : UXMLWidget {
		private const string FORMAT_SEARCH = "{}";
		private static readonly Identifier FPS_ELEMENT = Identifier.Of("soulbound:metrics_hud/fps");
		private static readonly Identifier FRAME_TIME_ELEMENT = Identifier.Of("soulbound:metrics_hud/frame_time");
		private static readonly Identifier TOTAL_MEMORY_ELEMENT = Identifier.Of("soulbound:metrics_hud/total_memory");
		private static readonly Identifier GPU_MEMORY_ELEMENT = Identifier.Of("soulbound:metrics_hud/gpu_memory");
		private static readonly Identifier GC_ALLOC_ELEMENT = Identifier.Of("soulbound:metrics_hud/gc_alloc");
		private static readonly Identifier TPS_ELEMENT = Identifier.Of("soulbound:metrics_hud/tps");
		private static readonly Identifier TICK_TIME_ELEMENT = Identifier.Of("soulbound:metrics_hud/tick_time");
		private readonly DebugMetricsService metricsService;
		private readonly AverageCounter fpsCounter = new(10);
		private readonly AverageCounter frameTimeCounter = new(10);
		private readonly AverageCounter tpsCounter = new(60);
		private readonly AverageCounter tickTimeCounter = new(60);
		private MetricBinding[] metrics = Array.Empty<MetricBinding>();
		private readonly SoulboundUnityClient client;

		public MetricsHUD(DebugMetricsService metricsService, SoulboundUnityClient client) {
			this.metricsService = metricsService;
			this.client = client;
		}

		public static void CreateRoot(VisualElement parent) {
			VisualTreeAsset asset = AssetManager.Resolve<VisualTreeAsset>(new AssetKey("MetricsHUD"));
			asset.CloneTree(parent);
		}

		public override void OnBind(VisualElement root) {
			base.OnBind(root);
			this.metrics = this.CreateMetricBindings(root);
		}

		internal void Tick() {
			if (GameSettings.keybinds.toggleDebugMetrics.WasPressed()) {
				if (!this.isVisible) {
					this.Show();
					this.client.ShowChunkFeatures(true);
				} else {
					this.Hide();
					this.client.ShowChunkFeatures(false);
				}
			}
		}

		private MetricBinding[] CreateMetricBindings(VisualElement root) {
			return new[] {
				new LabelMetricBinding(root, FPS_ELEMENT, (data, format) => {
					return FormatOutput(format, 
						$"avg: {this.fpsCounter.GetAverage():F1}, " +
						$"min: {this.fpsCounter.GetMin():F1}, " +
						$"max: {this.fpsCounter.GetMax():F1}, " +
						$"curr: {this.fpsCounter.GetCurrent():F1}"
					);
				}),
				new LabelMetricBinding(root, FRAME_TIME_ELEMENT, (data, format) => {
					return FormatOutput(format,
						$"avg: {this.frameTimeCounter.GetAverage():F1}ms, " +
						$"min: {this.frameTimeCounter.GetMin():F1}ms, " +
						$"max: {this.frameTimeCounter.GetMax():F1}ms, " +
						$"curr: {this.frameTimeCounter.GetCurrent():F1}ms"
					);
				}),
				new LabelMetricBinding(root, TOTAL_MEMORY_ELEMENT, (data, format) => {
					return FormatOutput(format,
						FormatFloat(ReadFloat(data, DebugMetricId.TotalManagedMemory), v => $"{v:F1}MB"),
						FormatFloat(ReadFloat(data, DebugMetricId.MonoHeap), v => $"{v:F1}MB")
					);
				}),
				new LabelMetricBinding(root, GPU_MEMORY_ELEMENT,(data, format) => {
					return FormatOutput(format,
						FormatFloat(ReadFloat(data, DebugMetricId.GpuManagedMemory), v => $"{v:F1}MB"),
						FormatFloat(ReadFloat(data, DebugMetricId.GpuReservedMemory), v => $"{v:F1}MB")
					);
				}),
				new LabelMetricBinding(root, GC_ALLOC_ELEMENT,
					(data, format) => FormatOutput(format, FormatFloat(ReadFloat(data, DebugMetricId.GcAlloc), v => $"{v:F0}B"))
				),
				new LabelMetricBinding(root, TPS_ELEMENT, (data, format) => {
					return FormatOutput(format, 
						$"{this.tpsCounter.GetCurrent()}, " +
						$"min: {this.tpsCounter.GetMin():F1}, " +
						$"max: {this.tpsCounter.GetMax():F1}, " +
						$"avg: {this.tpsCounter.GetAverage():F1}"
					);
				}),
				new LabelMetricBinding(root, TICK_TIME_ELEMENT, (data, format) => {
					return FormatOutput(format,
						$"{this.tickTimeCounter.GetCurrent():F1}ms, " +
						$"min: {this.tickTimeCounter.GetMin():F1}ms, " +
						$"max: {this.tickTimeCounter.GetMax():F1}ms, " +
						$"avg: {this.tickTimeCounter.GetAverage():F1}ms"
					);
				})
			};
		}

		private static string FormatOutput(string format, params string[] args) {
			string output = format;
			foreach (string arg in args) {
				output = output.ReplaceFirst(FORMAT_SEARCH, arg);
			}
			return output;
		}

		private static string FormatFloat(float? value, Func<float, string> transformer, string fallback = "N/A") {
			return value is not { } nonNull ? fallback : transformer(nonNull);
		}

		public void Refresh() {
			if (!this.isVisible) return;
			DebugMetricsSnapshot data = this.metricsService.CaptureData();

			TickIfExistent(data, DebugMetricId.Fps, ReadFloat, this.fpsCounter.Tick);
			TickIfExistent(data, DebugMetricId.FrameTime, ReadFloat, this.frameTimeCounter.Tick);
			TickIfExistent(data, DebugMetricId.Tps, ReadInt, this.tpsCounter.Tick);
			TickIfExistent(data, DebugMetricId.TickTime, ReadFloat, this.tickTimeCounter.Tick);
			for (int i = 0; i < this.metrics.Length; i++) {
				this.metrics[i].Refresh(data);
			}
		}

		private static void TickIfExistent<T>(DebugMetricsSnapshot data, DebugMetricId id, Func<DebugMetricsSnapshot, DebugMetricId, T?> valueSupplier, Action<T> consumer) where T : struct {
			if (valueSupplier(data, id) is { } value) {
				consumer(value);
			}
		}

		private static float? ReadFloat(DebugMetricsSnapshot data, DebugMetricId id) {
			return Read(data, id) as float?;
		}

		private static int? ReadInt(DebugMetricsSnapshot data, DebugMetricId id) {
			return Read(data, id) as int?;
		}

		private static object? Read(DebugMetricsSnapshot data, DebugMetricId id) {
			return data.TryGet(id, out object value) ? value : null;
		}
	}
}
