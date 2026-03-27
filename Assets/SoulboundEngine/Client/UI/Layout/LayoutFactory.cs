using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace SoulboundEngine.Client.UI.Layouts {
	public sealed class LayoutFactory {
		public HorizontalOrVerticalLayout<VerticalLayoutGroup> Vertical() => new();
		public HorizontalOrVerticalLayout<HorizontalLayoutGroup> Horizontal() => new();
		public GridLayout Grid() => new();
	}
}
