using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI {
	public class TransitStackHandle : MonoBehaviour, ITransitStackHandle {
		private ItemDisplay display;

		public void Init(ItemDisplay display) {
			this.display = display;
			display.SetRaycastTarget(false);
		}

		[PROTOTYPICAL]
		[Obsolete]
		private void Update() {
			display.transform.position = UnityEngine.Input.mousePosition;
		}

		void ITransitStackHandle.Destroy() => display.Destroy();

		ItemStack ITransitStackHandle.GetStack() => display.stack;
	}
}
