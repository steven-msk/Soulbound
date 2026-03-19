using SoulboundBackend.Client.UI;
using SoulboundBackend.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SoulboundBackend.Client.Debug.Metrics.View {
	public sealed class MetricsHUD  : ToggleableOverlay<UIOverlayNode>, IDebugMetricsProvider {
		private readonly DebugMetricsService metricsService;

		public MetricsHUD(DebugMetricsService metricsService) {
			this.metricsService = metricsService;
		}

		protected override UIOverlayNode GetNode() {
			GameObject obj = new("Metrics HUD", typeof(RectTransform));
			VerticalLayoutGroup layout = obj.AddComponent<VerticalLayoutGroup>();
			layout.childControlWidth = layout.childControlHeight = false;
			layout.childForceExpandWidth = layout.childForceExpandHeight = false;

			RectTransform rect = obj.GetComponent<RectTransform>();
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector2.one;
			rect.pivot = new Vector2(0.5f, 0.5f);
			rect.sizeDelta = Vector2.zero;

			MetricsHUDHandle handle = obj.AddComponent<MetricsHUDHandle>();
			handle.Init(this);
			return new UIOverlayNode(obj);
		}

		DebugMetricsSnapshot IDebugMetricsProvider.GetData() {
			return metricsService.CaptureData();
		}
	}
}
