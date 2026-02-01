using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace SoulboundBackend.Client.UI {
	public struct LayoutGroupOptions {
		public RectOffset padding;
		public float spacing;
		public TextAnchor childAlignment;
		public bool reverseArrangement;
		public bool2 controlChildSize;
		public bool2 useChildScale;
		public bool2 childForceExpand;

		public readonly void Apply(HorizontalOrVerticalLayoutGroup group) {
			group.padding = padding;
			group.spacing = spacing;
			group.childAlignment = childAlignment;
			group.reverseArrangement = reverseArrangement;
			group.childControlWidth = controlChildSize.x;
			group.childControlHeight = controlChildSize.y;
			group.childScaleWidth = useChildScale.x;
			group.childScaleHeight = useChildScale.y;
			group.childForceExpandWidth = childForceExpand.x;
			group.childForceExpandHeight = childForceExpand.y;
		}
	}
}
