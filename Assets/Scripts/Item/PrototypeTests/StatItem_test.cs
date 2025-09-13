using System;
using System.Collections.Generic;

public class StatItem_test : StatItem {
	public override bool applyInstantStatsOnHoverOrSelect => throw new NotImplementedException();

	public override string name => throw new NotImplementedException();

	public override ItemAspect aspect => throw new NotImplementedException();

	public override int maxStackSize => throw new NotImplementedException();

	public override IEnumerable<StatMapping> statMappings => throw new NotImplementedException();

	protected override Func<Item, TooltipData> tooltipSupplier => throw new NotImplementedException();

	protected override TooltipRenderer.NodeStyleProvider nodeStyleProvider => throw new NotImplementedException();
}
