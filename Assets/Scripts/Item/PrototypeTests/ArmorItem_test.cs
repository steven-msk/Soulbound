using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable

public sealed class ArmorItem_test : ArmorItem {
	public override ArmorType armorType => ArmorType.Boots;

	public override List<AbstractSerializableStat> stats => new() {
		new SerializableStat<int>(StatDefinition<int>.MaxHealth, 1, StatApplicationType.Flat, true)
	};

	public override string name => "armorItem_test";

	public override Sprite icon => ResourceManager.Get<Sprite, ResourceGroups.Items.Icons>("chestplate_overlay");

	public override Func<GameObject> worldPrefabSupplier => null;

	public override int maxStackSize => 1;

	protected override Func<Item, TooltipData> tooltipSupplier => (item) => {
		return new TooltipData.Builder()
			.AddNode(TooltipNode.Title, "ArmorItem_test")
			.Finish();
	};

	protected override TooltipRenderer.NodeStyleProvider? nodeStyleProvider => null;
}
