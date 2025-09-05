using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable

public sealed class ArmorItem_test : ArmorItem {
	public override ArmorType armorType => ArmorType.Chestplate;

	public override List<AbstractSerializableStat> stats => new() {
		new SerializableStat<int>(StatDefinition<int>.MaxHealth, 1, StatApplicationType.Flat, true)
	};

	public override string name => "armorItem_test";

	public override ItemAspect aspect => ItemAspect.Simple("chestplate_overlay");

	public override int maxStackSize => 1;

	protected override Func<Item, TooltipData> tooltipSupplier => (item) => {
		return new TooltipData.Builder()
			.AddNode(TooltipNode.Title, "ArmorItem_test")
			.Finish();
	};

	protected override TooltipRenderer.NodeStyleProvider? nodeStyleProvider => null;
}
