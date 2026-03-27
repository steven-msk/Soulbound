using SoulboundEngine.Common;
using SoulboundEngine.Common.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SoulboundEngine.Client.UI.Layouts {
	public class GridLayout : IUILayoutController, IGridLayoutBuilder {
		private RectOffset padding = new(0, 0, 0, 0);
		private Vector2 cellSize = new(100f, 100f);
		private Vector2 spacing = Vector2.zero;
		private TextAnchor childAlignment = TextAnchor.UpperLeft;
		private GridLayoutGroup.Corner startCorner = GridLayoutGroup.Corner.UpperLeft;
		private GridLayoutGroup.Axis startAxis = GridLayoutGroup.Axis.Horizontal;
		private GridLayoutGroup.Constraint constraint = GridLayoutGroup.Constraint.Flexible;
		private int constraintCount = 0;

		void IUILayoutController.ApplyTo(GameObject obj) {
			GridLayoutGroup group = obj.GetOrAddComponent<GridLayoutGroup>();
			group.padding = padding;
			group.cellSize = cellSize;
			group.spacing = spacing;
			group.childAlignment = childAlignment;
			group.startCorner = startCorner;
			group.startAxis = startAxis;
			group.constraint = constraint;
			group.constraintCount = constraintCount;
		}

		public GridLayout CellSize(Vector2 cellSize) {
			this.cellSize = cellSize;
			return this;
		}

		public GridLayout Padding(RectOffset padding) {
			this.padding = padding;
			return this;
		}

		public GridLayout Spacing(Vector2 spacing) {
			this.spacing = spacing;
			return this;
		}

		public GridLayout Flow(GridFlow flow) {
			startCorner = flow switch {
				GridFlow.LeftToRight_TopToBottom => GridLayoutGroup.Corner.UpperLeft,
				GridFlow.LeftToRight_BottomToTop => GridLayoutGroup.Corner.LowerLeft,
				GridFlow.RightToLeft_TopToBottom => GridLayoutGroup.Corner.UpperRight,
				GridFlow.RightToLeft_BottomToTop => GridLayoutGroup.Corner.LowerRight,
				_ => throw new NotImplementedException()
			};
			return this;
		}

		public GridLayout FlowPattern(GridFlowPattern pattern) {
			startAxis = pattern switch {
				GridFlowPattern.FillHorizontal => GridLayoutGroup.Axis.Horizontal,
				GridFlowPattern.FillVertical => GridLayoutGroup.Axis.Vertical,
				_ => throw new NotImplementedException()
			};
			return this;
		}

		public GridLayout ContentAlignment(TextAnchor alignment) {
			childAlignment = alignment;
			return this;
		}

		public GridLayout Fixed(GridLayoutGroup.Constraint constraint, int count) {
			if (constraint == GridLayoutGroup.Constraint.Flexible) {
				UnityEngine.Debug.Log("Grid layout is already flexible");
				return this;
			}
			this.constraint = constraint;
			constraintCount = count;
			return this;
		}

		void IUILayoutController.OnChildAdded(UIElementNode node) {
		}

		void IUILayoutController.OnChildRemoved(UIElementNode node) {
		}
	}
}
