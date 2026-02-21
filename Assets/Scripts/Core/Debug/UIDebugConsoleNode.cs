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
		public readonly ScrollRect scrollRect;
		public readonly RectTransform contentRect;

		public UIDebugConsoleNode(GameObject gameObject, IDebugConsoleHandle handle, ScrollRect scrollRect, RectTransform contentRect)
			: base(gameObject) {
			this.handle = handle;
			this.scrollRect = scrollRect;
			this.contentRect = contentRect;
		}
	}
}
