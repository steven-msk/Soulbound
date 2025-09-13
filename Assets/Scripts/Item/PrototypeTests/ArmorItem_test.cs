using System;
using System.Collections.Generic;

#nullable enable

public sealed class ArmorItem_test : ArmorItem {
	public override ArmorType armorType => ArmorType.Chestplate;

	public override IEnumerable<StatMapping> statMappings { get; }

	public override string name => "armorItem_test";

	public override ItemAspect aspect => _aspect;
	private readonly ItemAspect _aspect = ItemAspect.Simple("chestplate_overlay");

	public override int maxStackSize => 1;

	protected override Func<Item, TooltipData> tooltipSupplier { get; }

	protected override TooltipRenderer.NodeStyleProvider? nodeStyleProvider => null;

	public ArmorItem_test() {
		StatMappingBuilder mappingBuilder = new StatMappingBuilder()
			.SetStats(() => new DynamicMap<AbstractSerializableStat>() {
				["maxHealth"] = new SerializableStat<int>(StatDefinition.MaxHealth, 1, showAsBonus: true),
				["defense"] = new SerializableStat<int>(StatDefinition.Defense, 10, showAsBonus: true)
			})
			.WithTooltipNodes((stats) => new List<TooltipNodeData>() {
				new TooltipNodeData(TooltipNode.Stats, stats["defense"].ToString()),
				new TooltipNodeData(TooltipNode.Stats, $"When equipped, gain {stats["maxHealth"]} for 5s")
			})
			.BindEffectHandlers((stats) => new DynamicMap<IStatEffectHandler>() {
				["healthHandler"] = IStatEffectHandler.Timed(5f, this, stats["maxHealth"]),
				["defenseHandler"] = IStatEffectHandler.Static(this, stats["defense"])
			})
			.BindActivators((handlers) => new List<StatActivator>() {
				new StatActivator(
					activationBinder: callback => onEquipped += callback,
					deactivationBinder: callback => onUnequipped += callback,
					handlers["healthHandler"], handlers["defenseHandler"])
			});
		this.statMappings = mappingBuilder.ResolveMappings();
		this.tooltipSupplier = (item) => new TooltipData.Builder()
			.AddNode(TooltipNode.Title, this.name)
			.AddNodes(mappingBuilder.ResolveTooltipNodes())
			.Finish();
	}
}
