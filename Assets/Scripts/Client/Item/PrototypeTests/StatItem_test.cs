using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.Stats;
using SoulboundBackend.Client.UI.Tooltip;
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
		this.statMappings = new List<StatMapping>();
		this.tooltipSupplier = null;
	}
}
