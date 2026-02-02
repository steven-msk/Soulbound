using SoulboundBackend.Client.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using GridLayout = SoulboundBackend.Client.UI.GridLayout;

namespace Assets.Scripts.Client.UI.Layout {
	public interface IGridLayoutBuilder {
		GridLayout Padding(RectOffset padding);
		GridLayout CellSize(Vector2 cellSize);
		GridLayout Spacing(Vector2 spacing);
		GridLayout ContentAlignment(TextAnchor alignment);
		GridLayout Fixed(GridLayoutGroup.Constraint constraint, int count);
		GridLayout Flow(GridFlow flow);
		GridLayout FlowPattern(GridFlowPattern pattern);
	}
}
