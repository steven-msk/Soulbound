using SoulboundBackend.Client.UI.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.UI {
	public sealed class HotbarSlotHandle : InventorySlotHandle {
		public void ApplyFadedLayout(bool fadedLayout) {
			UnityEngine.Debug.Log("apply faded layout:" + fadedLayout);
		}

		public void SetMainSlotLayout() {
			UnityEngine.Debug.Log("apply main slot layout");
		}

		public void RemoveMainSlotLayout() {
			UnityEngine.Debug.Log("remove main slot layout");
		}
	}
}
