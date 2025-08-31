using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

public class TooltipData {
	public List<TooltipNodeData> nodes { get; private set; } = new();
	public CompoundTooltipLayout layout { get; private set; }

	public TooltipData(IEnumerable<TooltipNodeData> nodes, CompoundTooltipLayout? layout = null) {
		this.nodes.AddRange(nodes);
		this.layout = layout ?? CompoundTooltipLayout.Default();
	}
}
