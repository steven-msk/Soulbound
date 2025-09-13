using SoulboundBackend.Common;
using UnityEngine;
using UnityEngine.UI;


namespace SoulboundBackend.Client.UI.Tooltip {
	public class CompoundTooltipLayout {
		public RectOffset padding = new(10, 10, 8, 8);
		public float spacing = 4;
		public TextAnchor childAlignment = TextAnchor.UpperCenter;
		public bool reverseArrangement = false;
		public Bool2 controlChildSize = Bool2.True;
		public Bool2 useChildScale = Bool2.False;
		public Bool2 childForceExpand = Bool2.True;

		public void Apply(VerticalLayoutGroup layout) {
			layout.padding = padding;
			layout.spacing = spacing;
			layout.childAlignment = childAlignment;
			layout.reverseArrangement = reverseArrangement;
			layout.childControlWidth = controlChildSize.x;
			layout.childControlHeight = controlChildSize.y;
			layout.childScaleWidth = useChildScale.x;
			layout.childScaleHeight = useChildScale.y;
			layout.childForceExpandWidth = childForceExpand.x;
			layout.childForceExpandHeight = childForceExpand.y;
		}

		public static CompoundTooltipLayout SpacingOnly(float spacing) => new() { spacing = spacing };

		public static CompoundTooltipLayout Default() => new();
	}
}
