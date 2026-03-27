using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.ItemSystem.View;
using SoulboundEngine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundEngine.Client.ItemSystem.Container.View {
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
