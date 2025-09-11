using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks; 
using UnityEngine;

#nullable enable

public sealed class ArmorItem_test : ArmorItem {
	public override ArmorType armorType => ArmorType.Chestplate;

	public override IEnumerable<StatMapping> statMappings { get; }

	public override string name => "armorItem_test";

	public override ItemAspect aspect => _aspect;
	private ItemAspect _aspect = ItemAspect.Simple("chestplate_overlay");

	public override int maxStackSize => 1;

	protected override Func<Item, TooltipData> tooltipSupplier => (item) => {
		return new TooltipData.Builder()
			.AddNode(TooltipNode.Title, "ArmorItem_test")
			.Finish();
	};

	protected override TooltipRenderer.NodeStyleProvider? nodeStyleProvider => null;

	public ArmorItem_test() {
		statMappings = new StatMappingBuilder()
			.SetStats(() => {
				return new DynamicMap<AbstractSerializableStat>() {
					["maxHealth"] = new SerializableStat<int>(StatDefinition.MaxHealth, 1, StatApplicationType.Flat, true),
					["defense"] = new SerializableStat<int>(StatDefinition.Defense, 10, StatApplicationType.Flat, true)
				};
			})
			.BindEffectHandlers((stats) => {
				return new DynamicMap<IStatEffectHandler>() {
					["healthHandler"] = IStatEffectHandler.Timed(5f, this, stats["maxHealth"]),
					["defenseHandler"] = IStatEffectHandler.Static(this, stats["defense"])
				};
			})
			.BindActivators((handlers) => {
				return new List<IStatActivator>() { new StatActivator_test(handlers["healthHandler"]), 
													new StatActivator_test(handlers["defenseHandler"])};
			})
			.ResolveMappings();
	}
}
