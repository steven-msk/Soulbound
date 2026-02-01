using Assets.Scripts.Client.UI.Layout;
using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SoulboundBackend.Client.UI {
	public class GridLayout : IUILayoutController, IGridLayoutBuilder {
		private RectOffset padding = new(0, 0, 0, 0);
		private Vector2 cellSize = new(100f, 100f);
		private Vector2 spacing = Vector2.zero;
		private TextAnchor childAlignment = TextAnchor.UpperLeft;
		private GridLayoutGroup.Corner startCorner = GridLayoutGroup.Corner.UpperLeft;
		private GridLayoutGroup.Axis startAxis = GridLayoutGroup.Axis.Horizontal;
		private GridLayoutGroup.Constraint constraint = GridLayoutGroup.Constraint.Flexible;

		void IUILayoutController.ApplyTo(GameObject obj) {
			GridLayoutGroup group = obj.GetOrAddComponent<GridLayoutGroup>();
			group.padding = padding;
			group.cellSize = cellSize;
			group.spacing = spacing;
			group.childAlignment = childAlignment;
			group.startCorner = startCorner;
			group.startAxis = startAxis;
			group.constraint = constraint;
		}

		public GridLayout CellSize(Vector2 cellSize) {
			this.cellSize = cellSize;
			return this;
		}

		public GridLayout ChildAlignment(TextAnchor childAlignment) {
			this.childAlignment = childAlignment;
			return this;
		}

		public GridLayout Constraint(GridLayoutGroup.Constraint constraint) {
			this.constraint = constraint;
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

		public GridLayout StartAxis(GridLayoutGroup.Axis startAxis) {
			this.startAxis = startAxis;
			return this;
		}

		public GridLayout StartCorner(GridLayoutGroup.Corner startCorner) {
			this.startCorner = startCorner;
			return this;
		}

		void IUILayoutController.OnChildAdded(UIElementNode node) {
		}
	}
}
