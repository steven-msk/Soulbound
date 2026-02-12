using SoulboundBackend.Client.ItemSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI {
	public record UITransitStackNode : UIElementNode {
		public readonly ITransitStackHandle handle;

		public UITransitStackNode(GameObject gameObject, ITransitStackHandle handle)
			: base(gameObject) {
			this.handle = handle;
		}
	}
}
