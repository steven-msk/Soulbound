using SoulboundBackend.Client.UI;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Debug;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Core.Debug {
	public sealed class MetricsHUD  : IDebugMetricsProvider {
		private readonly DebugMetricsService metricsService;
		private bool visible;
		private UIOverlayNode node;

		public MetricsHUD(DebugMetricsService metricsService) {
			this.metricsService = metricsService;
		}

		public void Toggle() {
			visible = !visible;
			CreateNodeIfNull();

			if (visible) node.Show();
			else node.Hide();
		}

		private void CreateNodeIfNull() {
			if (node != null) return;

			node = CreateNode();
			node.onDestroy += () => {
				node = null;
				visible = false;
			};
			Soulbound.instance.GetUIHandler().AddOverlay(node);
		}

		private UIOverlayNode CreateNode() {
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
