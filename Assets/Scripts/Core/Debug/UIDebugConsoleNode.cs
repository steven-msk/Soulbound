using SoulboundBackend.Client.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SoulboundBackend.Core.Debug {
	public sealed record UIDebugConsoleNode : UIOverlayNode {
		public readonly IDebugConsoleHandle handle;

		public UIDebugConsoleNode(GameObject gameObject, IDebugConsoleHandle handle)
			: base(gameObject) {
			this.handle = handle;
		}
	}
}
