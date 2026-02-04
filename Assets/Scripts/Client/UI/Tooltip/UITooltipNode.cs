using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI {
	public record UITooltipNode(GameObject gameObject) : UIElementNode(gameObject) {
		public ITooltipHandle handle { get; }

		public UITooltipNode(GameObject gameObject, ITooltipHandle handle)
			: this(gameObject) => this.handle = handle;
	}
}
