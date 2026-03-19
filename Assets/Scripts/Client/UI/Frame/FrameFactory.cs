using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI.Frames {
	public sealed class FrameFactory {
		public IUIFrame Stretch() => new StretchFrame();

		public IUIFrame StretchWithPadding(float padding) {
			return new StretchFrame(
				offsetMin: new Vector2(padding, padding),
				offsetMax: new Vector2(-padding, -padding)
			);
		}

		public IUIFrame StretchTop() => new StretchFrame(pivot: new Vector2(0.5f, 1f));
	}
}
