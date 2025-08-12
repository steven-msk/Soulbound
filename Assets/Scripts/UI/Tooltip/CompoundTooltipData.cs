using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class CompoundTooltipData {
	public IList<TooltipData> tooltips;
	public CompoundTooltipLayout layout;

	public CompoundTooltipData(CompoundTooltipLayout layout, params TooltipData[] tooltips) : this(tooltips, layout) { 
	} 

	public CompoundTooltipData(IList<TooltipData> tooltips, CompoundTooltipLayout layout) {
		this.tooltips = tooltips;
		this.layout = layout;
	}

	public AbstractTooltip Generate() => new CompoundTooltip(layout, tooltips.ToArray());
}