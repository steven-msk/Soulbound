using UnityEngine;
using UnityEngine.UI;

namespace SoulboundBackend.Client.UI.Layouts {
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
