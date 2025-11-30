using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.Stats;
using SoulboundBackend.Client.UI.Tooltip;
using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public sealed class ConsumableStatItem_test : ConsumableStatItem {
	public override IConsumable.ConsumeAction consumeAction => null;

	public override string name => "ConsumableStatItem_test";

	public override ItemAspect aspect => ItemAspectRegistry.Get(this, () => ItemAspect.Simple("fruit_icon"));

	public override int maxStackSize => Item.DEFAULT_MAX_STACK;

	public override IEnumerable<StatMapping> statMappings { get; }

	protected override Func<Item, TooltipData> tooltipSupplier { get; }

	protected override TooltipRenderer.NodeStyleFactory nodeStyleProvider => null;

	public override IConsumptionRestriction restriction { get; }

	public ConsumableStatItem_test() {
		StatMappingBuilder mappingBuilder = new StatMappingBuilder()
			.SetStats(() => new DynamicMap<AbstractValueModifier>() {
				["luck"] = new ValueModifier<float>(StatDefinition.Luck, 0.1f, true, StatApplicationType.Percentage)
			})
			.WithTooltipNodes(stats => new List<TooltipNodeData>() {
				new TooltipNodeData(TooltipNode.Stats, $"Grants {stats["luck"]} for 3s.")
			})
			.BindEffectHandlers(stats => new DynamicMap<IStatEffectHandler>() {
				["luckHandler"] = IStatEffectHandler.Timed(3f, resetOnEnable: true, this, stats["luck"])
			})
			.BindActivator(handlers => new StatActivator(
				activationBinder: callback => onConsumed += callback,
				deactivationBinder: null,
				handlers["luckHandler"]
			));
		this.statMappings = mappingBuilder.ResolveMappings();
		this.tooltipSupplier = item => new TooltipData.Builder()
			.AddNode(TooltipNode.Title, this.name)
			.AddNodes(mappingBuilder.ResolveTooltipNodes())
			.Finish();
		this.restriction = null;
	}
}
