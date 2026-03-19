using SoulboundBackend.Client.UI;
using SoulboundBackend.Client.UI.Frames;
using SoulboundBackend.Client.UI.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI.Containers {
	public sealed class ContainerBuilder {
		private readonly IUILayoutController layout;
		private readonly IUIFrame frame;
		private bool built;

		public ContainerBuilder(IUIFrame frame, IUILayoutController layout) {
			this.frame = frame;
			this.layout = layout;
		}

		public IUIElementContainer Build(IUIElementContainer container) {
			if (built) throw new InvalidOperationException("Container already built");
			built = true;

			GameObject obj = new("Container", typeof(RectTransform));
			UIContainerNode node = new(obj, container, layout, frame);
			container.AddElement(node);

			return node;
		}
	}
}
