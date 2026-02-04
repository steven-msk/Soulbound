using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI {
	public record UITooltipNode(GameObject gameObject) : UIElementNode(gameObject) {
		public ITooltipHandle handle { get; private set; }
		public bool isAlive { get; private set; }

		public UITooltipNode(GameObject gameObject, ITooltipHandle handle)
			: this(gameObject) {
			this.handle = handle;
			handle.onDestroyed += () => isAlive = false;
		}

	}
}
