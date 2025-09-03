using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BlockItem_test : BlockItem {
	public override Func<Block> blockGetter => throw new NotImplementedException();

	public BlockItem_test(string name, Sprite icon, Func<GameObject> worldPrefabSupplier, int maxStackSize, Func<Block> blockGetter)
		: base(name, icon, worldPrefabSupplier, maxStackSize, blockGetter) {
	}

	public BlockItem_test(string name, Sprite icon, Func<GameObject> worldPrefabSupplier, int maxStackSize, Func<Block> blockGetter,
		Func<Item, TooltipData> tooltipSupplier, TooltipRenderer.NodeStyleProvider nodeStyleProvider = null)
		: base(name, icon, worldPrefabSupplier, maxStackSize, blockGetter, tooltipSupplier, nodeStyleProvider) {
	}
}