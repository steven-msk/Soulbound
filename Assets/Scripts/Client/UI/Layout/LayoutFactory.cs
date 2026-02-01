using Assets.Scripts.Client.UI.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace SoulboundBackend.Client.UI {
	public sealed class LayoutFactory {
		public HorizontalOrVerticalLayout<VerticalLayoutGroup> Vertical() => new();
		public HorizontalOrVerticalLayout<HorizontalLayoutGroup> Horizontal() => new();
		public GridLayout Grid() => new();
	}
}
