using GluonGui.WorkspaceWindow.Views.WorkspaceExplorer.Explorer;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.Stats;
using SoulboundBackend.Client.UI.Tooltip;
using SoulboundBackend.Common;
using System;
using System.Collections.Generic;

public class ConsumableStatItem_test : ConsumableStatItem {
	public override IConsumable.ConsumeAction consumeAction => null;

	public override string name => "ConsumableStatItem_test";

	public override ItemAspect aspect => ItemAspectRegistry.Get(this, ItemAspect.Simple("fruit_icon"));

	public override int maxStackSize => Item.DEFAULT_MAX_STACK;

	public override IEnumerable<StatMapping> statMappings { get; }

	protected override Func<Item, TooltipData> tooltipSupplier { get; }

	protected override TooltipRenderer.NodeStyleProvider nodeStyleProvider => null;

	public override IEnumerable<IConsumptionRestriction> restrictions => _restrictions;
	private readonly IEnumerable<IConsumptionRestriction> _restrictions = IConsumptionRestriction.Single(new CooldownRestriction(5f));

	public ConsumableStatItem_test() {
		StatMappingBuilder mappingBuilder = new StatMappingBuilder()
			.SetStats(() => new DynamicMap<AbstractSerializableStat>() {
				["luck"] = new SerializableStat<float>(StatDefinition.Luck, 0.1f, StatApplicationType.Percentage)
			})
			.WithTooltipNodes(stats => new List<TooltipNodeData>() {
				new TooltipNodeData(TooltipNode.Stats, $"Grants {stats["luck"]} for 3s.")
			})
			.BindEffectHandlers(stats => new DynamicMap<IStatEffectHandler>() {
				["luckHandler"] = IStatEffectHandler.Timed(3f, this, stats["luck"])
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
	}

	public record Consumption(bool e);
}
