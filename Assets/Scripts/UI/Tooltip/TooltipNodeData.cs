using System;
using System.Linq;

public class TooltipNodeData {
	public string text { get; set; }
	public TooltipNode node { get; set; }

	public bool IsEmpty => string.IsNullOrEmpty(text);

	public TooltipNodeData(TooltipNode node, string text) {
		this.text = text;
		this.node = node;
	}

	public static TooltipNodeData[] Concat(TooltipNodeData[] first, TooltipNodeData[] second) => first.Concat(second).ToArray();

	public static TooltipNodeData[] Concat(TooltipNodeData[] first, TooltipNodeData second) => Concat(first, new TooltipNodeData[] { second });
}
