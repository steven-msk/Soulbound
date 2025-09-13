using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class TooltipNodeStylePresets {
	static readonly Dictionary<TooltipNode, TooltipNodeStyle> presets = new() {
		[TooltipNode.Title] = new TooltipNodeStyle() {
			fontSize = 12,
			fontStyle = FontStyles.Bold
		},

		[TooltipNode.Lore] = new TooltipNodeStyle() {
			fontStyle = FontStyles.Italic,
			textColor = new Color(0.75f, 0.75f, 0.75f)
		},
	};

	public static TooltipRenderer.NodeStyleProvider PresetProvider() {
		return GetPreset;
	}

	public static TooltipNodeStyle GetPreset(TooltipNode node) {
		if (presets.TryGetValue(node, out var style)) {
			return style;
		}
		return new TooltipNodeStyle();
	}
}