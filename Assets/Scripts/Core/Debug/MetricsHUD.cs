using SoulboundBackend.Client.UI;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Debug;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
			node.onDestroy += () => node = null;
			Soulbound.instance.GetUIHandler().AddOverlay(node);
		}

		private UIOverlayNode CreateNode() {
			GameObject obj = new("Metrics HUD", typeof(RectTransform));
			MetricsHUDHandle handle = obj.AddComponent<MetricsHUDHandle>();
			handle.Init(this);
			return new UIOverlayNode(obj);
		}

		DebugMetricsSnapshot IDebugMetricsProvider.GetData() {
			return metricsService.CaptureData();
		}
	}
}
