using SoulboundBackend.Common;
using SoulboundBackend.Common.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace SoulboundBackend.Client.UI.Layouts {
	public class HorizontalOrVerticalLayout<T> : IUILayoutController, IHorizontalOrVerticalLayoutBuilder<T> where T : HorizontalOrVerticalLayoutGroup {
		private RectOffset padding = new(0, 0, 0, 0);
		private float spacing = 0f;
		private TextAnchor childAlignment = TextAnchor.UpperCenter;
		private bool reverseArrangement = false;
		private bool2 controlChildSize = false;
		private bool2 useChildScale = true;
		private bool2 childForceExpand = false;

		void IUILayoutController.ApplyTo(GameObject obj) {
			var group = obj.GetOrAddComponent<T>();
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

		public HorizontalOrVerticalLayout<T> Padding(RectOffset padding) {
			this.padding = padding;
			return this;
		}

		public HorizontalOrVerticalLayout<T> ChildSizing(ChildSizingMode childSizingMode) {
			switch (childSizingMode) {
				case ChildSizingMode.Fixed:
					controlChildSize = false;
					childForceExpand = false;
					break;
				case ChildSizingMode.Preferred:
					controlChildSize = true;
					childForceExpand = false;
					break;
				case ChildSizingMode.Stretch:
					controlChildSize = true;
					childForceExpand = true;
					break;
			}
			return this;
		}

		public HorizontalOrVerticalLayout<T> ControlChildSize(bool2 controlChildSize) {
			this.controlChildSize = controlChildSize;
			return this;
		}

		public HorizontalOrVerticalLayout<T> ControlChildWidth(bool controlWidth) {
			controlChildSize.x = controlWidth;
			return this;
		}

		public HorizontalOrVerticalLayout<T> ContolChildHeight(bool controlHeight) {
			controlChildSize.y = controlHeight;
			return this;
		}

		public HorizontalOrVerticalLayout<T> Align(UIAlignment alignment) {
			childAlignment = alignment switch {
				UIAlignment.Start => typeof(T) == typeof(HorizontalLayoutGroup)
					? TextAnchor.MiddleLeft
					: TextAnchor.UpperCenter,
				UIAlignment.Center => TextAnchor.MiddleCenter,
				UIAlignment.End => typeof(T) == typeof(HorizontalLayoutGroup)
					? TextAnchor.MiddleRight
					: TextAnchor.LowerCenter,
				_ => throw new NotImplementedException(),
			};
			return this;
		}

		public HorizontalOrVerticalLayout<T> Align(TextAnchor alignment) {
			childAlignment = alignment;
			return this;
		}

		public HorizontalOrVerticalLayout<T> ChildForceExpand(bool2 childForceExpand) {
			this.childForceExpand = childForceExpand;
			return this;
		}

		public HorizontalOrVerticalLayout<T> ChildForceExpandWidth(bool forceExpandWidth) {
			childForceExpand.x = forceExpandWidth;
			return this;
		}

		public  HorizontalOrVerticalLayout<T> ChildForceExpandHeight(bool forceExpandHeight) {
			childForceExpand.y = forceExpandHeight;
			return this;
		}

		public HorizontalOrVerticalLayout<T> ReverseArrangement(bool reverseArrangement) {
			this.reverseArrangement	= reverseArrangement;
			return this;
		}

		public HorizontalOrVerticalLayout<T> Spacing(float spacing) {
			this.spacing = spacing;
			return this;
		}

		public HorizontalOrVerticalLayout<T> UseChildSize(bool2 useChildSize) {
			this.useChildScale = useChildSize;
			return this;
		}

		void IUILayoutController.OnChildAdded(UIElementNode node) {
		}

		void IUILayoutController.OnChildRemoved(UIElementNode node) {
		}
	}
}
