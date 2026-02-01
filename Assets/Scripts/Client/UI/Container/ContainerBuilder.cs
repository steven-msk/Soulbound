using SoulboundBackend.Client.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Client.UI.Container {
	public sealed class ContainerBuilder {
		private readonly Func<IUILayoutController> layoutFactory;
		private bool built;

		public ContainerBuilder(Func<IUILayoutController> layoutFactory) {
			this.layoutFactory = layoutFactory;
		}

		public IUIElementContainer Build(IUIElementContainer container) {
			if (built) throw new InvalidOperationException("Container already built");
			built = true;

			GameObject obj = new("Container", typeof(RectTransform));
			UIContainerNode node = new(obj, layoutFactory());
			container.AddElement(node);

			return node;
		}
	}
}
