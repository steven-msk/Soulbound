using System;
using System.Linq;

[Serializable]
public class TooltipData : ITooltipSerializer {
	public string Text { get; set; }
	public TooltipSectionLayout Layout { get; set; }

	public TooltipData(TooltipSectionLayout layout, string text) {
		Text = text;
		Layout = layout;
	}

	public AbstractTooltip Generate() => new Tooltip(this);

	public static TooltipData[] Concat(TooltipData[] first, TooltipData[] second) => first.Concat(second).ToArray();

	public static TooltipData[] Concat(TooltipData[] first, TooltipData second) => Concat(first, new TooltipData[] { second });
}
