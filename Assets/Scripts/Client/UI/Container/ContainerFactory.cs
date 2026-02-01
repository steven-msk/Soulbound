using Assets.Scripts.Client.UI.Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.UI {
	public sealed class ContainerFactory {
		public ContainerBuilder Vertical() => new(() => new VerticalLayoutController());
		public ContainerBuilder Horizontal() => new(() => new HorizontalLayoutController());
	}
}
