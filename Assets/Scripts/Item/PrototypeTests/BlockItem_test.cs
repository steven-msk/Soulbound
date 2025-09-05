using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BlockItem_test : BlockItem {
	public override Func<Block> blockGetter => throw new NotImplementedException();

	public BlockItem_test(string name, ItemAspect aspect, int maxStackSize, Func<Block> blockGetter)
		: base(name, aspect, maxStackSize, blockGetter) {
	}

	public BlockItem_test(string name, ItemAspect aspect, int maxStackSize, Func<Block> blockGetter,
		Func<Item, TooltipData> tooltipSupplier, TooltipRenderer.NodeStyleProvider nodeStyleProvider = null)
		: base(name, aspect, maxStackSize, blockGetter, tooltipSupplier, nodeStyleProvider) {
	}
}