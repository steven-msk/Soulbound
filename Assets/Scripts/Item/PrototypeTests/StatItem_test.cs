using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class StatItem_test : StatItem {
	public override bool applyInstantStatsOnHoverOrSelect => throw new NotImplementedException();

	public override string name => throw new NotImplementedException();

	public override ItemAspect aspect => throw new NotImplementedException();

	public override int maxStackSize => throw new NotImplementedException();

	public override IEnumerable<StatMapping> statMappings => throw new NotImplementedException();

	protected override Func<Item, TooltipData> tooltipSupplier => throw new NotImplementedException();

	protected override TooltipRenderer.NodeStyleProvider nodeStyleProvider => throw new NotImplementedException();
}
