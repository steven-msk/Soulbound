using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum TooltipSection {
	Title = 0,
	Tags,
	Stats,
	Affixes,
	Info,
	Lore,
	//..
}

public static class TooltipSectionLayoutPreset {
	static readonly Dictionary<TooltipSection, TooltipSectionLayout> tooltipSectionPresets = new() {
		[TooltipSection.Title] = new TooltipSectionLayout(TooltipSection.Title, applyPreset: false)	{
			fontSize = 24,
			fontStyle = FontStyles.Bold
		},

		[TooltipSection.Lore] = new TooltipSectionLayout(TooltipSection.Lore, applyPreset: false) {
			fontStyle = FontStyles.Italic,
			textColor = new Color(0.75f, 0.75f, 0.75f)
		},
	};

	public static void ApplyLayoutPreset(this TooltipSection section, TooltipSectionLayout layout) {
		TooltipSectionLayout preset = tooltipSectionPresets.GetValueOrDefault(section, layout);
		layout.fontStyle = preset.fontStyle;
		layout.alignment = preset.alignment;
		layout.fontSize = preset.fontSize;
		layout.fontAsset = preset.fontAsset;
		layout.margin = preset.margin;
		layout.textColor = preset.textColor;
	}

	public static TooltipSectionLayout GetDefaultLayout(this TooltipSection section) => new(section);
}
