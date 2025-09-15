using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.Stats;
using SoulboundBackend.Client.UI.Tooltip;
using SoulboundBackend.Common;
using System;
using System.Collections.Generic;

public class StatItem_test : StatItem {
	public override bool applyInstantStatsOnHoverOrSelect => false;

	public override string name => "StatItem_test";

	public override ItemAspect aspect => ItemAspectRegistry.Get(this, ItemAspect.Simple("gem_icon"));

	public override int maxStackSize => Item.CustomMaxStack(128);

	public override IEnumerable<StatMapping> statMappings { get; }

	protected override Func<Item, TooltipData> tooltipSupplier { get; }

	protected override TooltipRenderer.NodeStyleProvider nodeStyleProvider => null;

	public StatItem_test() {
		StatMappingBuilder mappingBuilder = new StatMappingBuilder()
			.SetStats(() => new DynamicMap<AbstractSerializableStat>() {
				["physicalDamage"] = new SerializableStat<int>(StatDefinition.PhysicalDamage, 10, StatApplicationType.Percentage, showAsBonus: true)
			})
			.WithTooltipNodes(stats => new List<TooltipNodeData>() {
				new TooltipNodeData(TooltipNode.Stats, $"While in inventory, gain {stats["physicalDamage"]}.")
			})
			.BindEffectHandlers(stats => new DynamicMap<IStatEffectHandler>() {
				["physicalDamageHandler"] = IStatEffectHandler.Static(this, stats["physicalDamage"])
			})
			.BindActivator(handlers => new StatActivator(
				handlers["physicalDamageHandler"]
			));
		this.statMappings = mappingBuilder.ResolveMappings();
		this.tooltipSupplier = item => new TooltipData.Builder()
			.AddNode(TooltipNode.Title, this.name)
			.AddNodes(mappingBuilder.ResolveTooltipNodes())
			.Finish();
	}
}
