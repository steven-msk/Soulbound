using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.ItemSystem {
    [Flags]
    public enum ToolType {
        None = 0,
        Pickaxe = 1 << 0,
        Shovel  = 1 << 1,
        Axe     = 1 << 2,
        How     = 1 << 3,
        Hammer  = 1 << 4,

        All = ~0
    }
}
