using SoulboundEngine.Client.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Logger = SoulboundEngine.Client.Debug.Logging.Logger;

namespace SoulboundEngine.Client.Debug.Metrics.View {
	public sealed class MetricsHUDHandle : MonoBehaviour {
		private IDebugMetricsProvider metricsProvider;
		private readonly AverageFrameCounter fpsCounter = new(10);
		private readonly AverageFrameCounter frameTimeCounter = new(10);
		private TextMeshProUGUI fps;
		private TextMeshProUGUI frameTime;
		private TextMeshProUGUI fixedUpdateTime;
		private TextMeshProUGUI totalMemory;
		private TextMeshProUGUI gpuMemory;
		private TextMeshProUGUI gcAlloc;

		public void Init(IDebugMetricsProvider metricsProvider) {
			this.metricsProvider = metricsProvider;

			fps = CreateMetricDisplay("FPS: 99999.9");
			frameTime = CreateMetricDisplay("Frame time: 99999.9ms");
			fixedUpdateTime = CreateMetricDisplay("Fixed update time: 99999.9ms");
			totalMemory = CreateMetricDisplay("Total Memory: 99999.9MB / 99999.9MB");
			gpuMemory = CreateMetricDisplay("GPU Memory: 99999.9MB / 99999.9MB");
			gcAlloc = CreateMetricDisplay("GC Alloc: 9999999B");
		}

		private void Update() {
			DebugMetricsSnapshot data = metricsProvider.GetData();

			fpsCounter.Tick(data.Get<float>("fps"));
			frameTimeCounter.Tick(data.Get<float>("frameTime"));

			fps.text = $"FPS: {fpsCounter.GetAverage():F1}";
			frameTime.text = $"Frame Time: {frameTimeCounter.GetAverage():F1}ms";
			fixedUpdateTime.text = $"Fixed Update Time: {data.Get<float>("fixedUpdateTime"):F1}ms";

			totalMemory.text = $"Total Memory: {data.Get<float>("totalManagedMemory"):F1}MB / {data.Get<float>("monoHeap"):F1}MB";
			gpuMemory.text = $"GPU Memory: {data.Get<float>("gpuManagedMemory"):F1}MB / {data.Get<float>("gpuReservedMemory"):F1}MB";
			gcAlloc.text = $"GC Alloc: {data.Get<float>("gcAlloc"):F0}B";
		}

		

		private TextMeshProUGUI CreateMetricDisplay(string setupText) {
			GameObject obj = new("Metric Display", typeof(RectTransform));
			obj.transform.SetParent(transform, false);

			TextMeshProUGUI text = obj.AddComponent<TextMeshProUGUI>();
			text.fontSize = 13f;
			text.text = setupText;

			LayoutElement layoutElement = obj.AddComponent<LayoutElement>();
			layoutElement.preferredWidth = text.preferredWidth;

			ContentSizeFitter fitter = obj.AddComponent<ContentSizeFitter>();
			fitter.horizontalFit = fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

			return text;
		}
	}
}
