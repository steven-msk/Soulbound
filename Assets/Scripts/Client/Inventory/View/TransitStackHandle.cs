using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI {
	public class TransitStackHandle : ITransitStackHandle {
		private readonly UIItemDisplay display;

		public TransitStackHandle(UIItemDisplay display) {
			this.display = display;
		}

		void ITransitStackHandle.Destroy() => display.Destroy();

		void ITransitStackHandle.SetDisplayPosition(Vector2 position) {
			display.SetPosition(position);
		}

		void ITransitStackHandle.SetDisplayParent(RectTransform parent) {
			display.SetParent(parent);
		}
	}
}
