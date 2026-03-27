using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundEngine.Client.UI {
	public record UIHandledOverlayNode<THandle> : UIOverlayNode {
		public readonly THandle handle;

		public UIHandledOverlayNode(GameObject gameObject, THandle handle)
			: base(gameObject) {
			this.handle = handle;
		}
	}
}
