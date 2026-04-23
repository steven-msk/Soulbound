using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundEngine.Client.UI.Tooltips {
	public record UITooltipNode : UIElementNode {
		public ITooltipHandle handle { get; private set; }
		public bool isAlive { get; private set; }

		public UITooltipNode(GameObject gameObject, ITooltipHandle handle)
			: base(gameObject) {
			this.handle = handle;
			handle.onDestroyed += () => isAlive = false;
		}

	}
}
