using SoulboundBackend.Client.ItemSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.BlockSystem {
    public class BreakRequirement {
        public int minBreakPower;
        public int explosionResistance;
        public ToolType compatibleTypes;

        public BreakRequirement(int minBreakPower, ToolType compatibleTypes, int explosionResistance) {
            this.minBreakPower = minBreakPower;
            this.compatibleTypes = compatibleTypes;
            this.explosionResistance = explosionResistance;
        }

        public bool CanBreakWith(IBreakingTool tool) {
            return tool.breakingPower >= minBreakPower && compatibleTypes.HasFlag(tool.types);
        }
    }
}
