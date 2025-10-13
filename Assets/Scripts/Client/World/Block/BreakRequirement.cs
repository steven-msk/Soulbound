using SoulboundBackend.Client.ItemSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.BlockSystem {
    public class BreakRequirement {
        public int minBreakPower;

        public BreakRequirement(int minBreakPower) {
            this.minBreakPower = minBreakPower;
        }

        public bool CanBreakWith(IBreakingTool tool) {
            return tool.breakingPower >= minBreakPower;
        }
    }
}
