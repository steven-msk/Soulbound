using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace SoulboundBackend.Client.UI {
	public interface IHorizontalOrVerticalLayoutBuilder<T> where T : HorizontalOrVerticalLayoutGroup {
		HorizontalOrVerticalLayout<T> Padding(RectOffset padding);
		HorizontalOrVerticalLayout<T> Spacing(float spacing);
		HorizontalOrVerticalLayout<T> ChildAlignment(TextAnchor childAlignment);
		HorizontalOrVerticalLayout<T> ReverseArrangement(bool reverseArrangement);
		HorizontalOrVerticalLayout<T> ControlChildSize(bool2 controlChildSize);
		HorizontalOrVerticalLayout<T> UseChildSize(bool2 useChildSize);
		HorizontalOrVerticalLayout<T> ChildForceExpand(bool2 childForceExpand);
	}
}
