using System;

[Serializable]
public class TooltipData : ITooltipSerializer {
	public string Text { get; set; }
	public TooltipSectionLayout Layout { get; set; }

	public TooltipData(TooltipSectionLayout layout, string text) {
		Text = text;
		Layout = layout;
	}

	public AbstractTooltip Generate() => new Tooltip(this);
}
