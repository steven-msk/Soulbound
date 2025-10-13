using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Tooltip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

public class ToolItem_test : StatItem_test, IBreakingTool {
    public int breakingPower => 1;
}
