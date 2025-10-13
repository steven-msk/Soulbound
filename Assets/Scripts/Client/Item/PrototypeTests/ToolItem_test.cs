using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Tooltip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

public class ToolItem_test : Item, IBreakingTool {
    public int breakingPower => 1;

    public override string name => "crappy pickaxe";

    public override ItemAspect aspect => ItemAspectRegistry.Get(this, () => ItemAspect.Simple("crappy_pickaxe"));

    public override int maxStackSize => 1;

    protected override Func<Item, TooltipData?> tooltipSupplier => item => new TooltipData.Builder().AddNode(TooltipNode.Title, item.name).Finish();

    protected override TooltipRenderer.NodeStyleProvider? nodeStyleProvider => null;
}
