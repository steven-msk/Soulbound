using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace SoulboundBackend.Client.UI.Tooltip {
	public class TooltipData {
		public List<TooltipNodeData> nodes { get; private set; } = new();
		public CompoundTooltipLayout layout { get; private set; }

		public TooltipData(IEnumerable<TooltipNodeData> nodes, CompoundTooltipLayout? layout = null) {
			this.nodes.AddRange(nodes);
			this.layout = layout ?? CompoundTooltipLayout.Default();
		}

		public void PurgeInvalidNodes() {
			List<TooltipNodeData> toRemove = new();
			foreach (var node in nodes) {
				if (string.IsNullOrEmpty(node.text)) {
					toRemove.Add(node);
				}
			}
			foreach (var node in toRemove) {
				this.nodes.Remove(node);
			}
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

			public Builder AddNodes(params TooltipNodeData[] nodes) {
				nodes.ToList().ForEach((node) => this.AddNode(node));
				return this;
			}

			public Builder AddNodes(params (TooltipNode node, string text)[] nodes) {
				return this.AddNodes(nodes.Select(entry => new TooltipNodeData(entry.node, entry.text)).ToArray());
			}

			public TooltipData Finish() => new TooltipData(nodes, layout);
		}
	}
}
