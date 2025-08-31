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

#nullable disable

	public class Builder {
		private List<TooltipNodeData> nodes = new();
		private CompoundTooltipLayout layout;

		public Builder(CompoundTooltipLayout layout = null) {
			this.layout = layout;
		}

		public Builder AddNode(TooltipNodeData node) {
			this.nodes.Add(node);
			return this;
		}

		public Builder AddNode(TooltipNode node, string text) {
			return this.AddNode(new TooltipNodeData(node, text));
		}

		public TooltipData Finish() => new TooltipData(nodes, layout);
	}
}
