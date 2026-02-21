using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable

namespace SoulboundBackend.Client.UI {
	public record UIOverlayNode : UIElementNode {
		public event Action? onDestroy;

		public UIOverlayNode(GameObject gameObject)
			: base(gameObject) {
		}

		public void Destroy() {
			onDestroy?.Invoke();
			GameObject.Destroy(gameObject);
		}
	}
}
