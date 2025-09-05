using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ConsumableStatItem_test : ConsumableStatItem {
	public override IConsumable.ConsumeAction consumeAction => throw new NotImplementedException();

	public override int consumeAmount => throw new NotImplementedException();

	public override List<AbstractSerializableStat> stats => throw new NotImplementedException();
	public override string name => throw new NotImplementedException();

	public override ItemAspect aspect => throw new NotImplementedException();

	public override int maxStackSize => throw new NotImplementedException();

	protected override Func<Item, TooltipData> tooltipSupplier => throw new NotImplementedException();

	protected override TooltipRenderer.NodeStyleProvider nodeStyleProvider => throw new NotImplementedException();
}
