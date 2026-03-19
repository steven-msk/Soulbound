using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace SoulboundBackend.Client.UI.Layouts {
	public interface IHorizontalOrVerticalLayoutBuilder<T> where T : HorizontalOrVerticalLayoutGroup {
		HorizontalOrVerticalLayout<T> Padding(RectOffset padding);
		HorizontalOrVerticalLayout<T> Spacing(float spacing);
		HorizontalOrVerticalLayout<T> Align(UIAlignment alignment);
		HorizontalOrVerticalLayout<T> ReverseArrangement(bool reverseArrangement);
		HorizontalOrVerticalLayout<T> ChildSizing(ChildSizingMode childSizingMode);
		HorizontalOrVerticalLayout<T> UseChildSize(bool2 useChildSize);
	}
}
