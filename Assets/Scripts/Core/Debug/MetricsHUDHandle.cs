using SoulboundBackend.Core.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Logger = SoulboundBackend.Core.Debug.Logging.Logger;

namespace SoulboundBackend.Core.Debug {
	public sealed class MetricsHUDHandle : MonoBehaviour {
		private IDebugMetricsProvider metricsProvider;

		public void Init(IDebugMetricsProvider metricsProvider) {
			this.metricsProvider = metricsProvider;
		}

		private void Update() {
			DebugMetricsSnapshot data = metricsProvider.GetData();
			Logger.LogInfo("fps: {}", data.Get("fps"));
		}
	}
}
