using SoulboundBackend.Client.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Client.UI.Container {
	public sealed class ContainerBuilder {
		private IUILayoutController layout;
		private IUIFrame frame;
		private bool built;

		public ContainerBuilder Layout(IUILayoutController layout) {
			this.layout = layout;
			return this;
		}

		public ContainerBuilder Frame(IUIFrame frame) {
			this.frame = frame;
			return this;
		}

		public IUIElementContainer Build(IUIElementContainer container) {
			if (built) throw new InvalidOperationException("Container already built");
			built = true;

			GameObject obj = new("Container", typeof(RectTransform));
			UIContainerNode node = new(obj, layout, frame);
			container.AddElement(node);

			return node;
		}
	}
}
