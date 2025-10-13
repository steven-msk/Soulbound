using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.ItemSystem {
    public interface IBreakingTool : IItemCapability {
        int breakingPower { get; }

        public void TryBreak(BlockPos pos, Level level, BreakSource source) {
            Block targetBlock = level.BlockAt(pos);
            if (targetBlock == Blocks.air) {
                return;
            }

            if (targetBlock.breakRequirement?.CanBreakWith(this) ?? false) {
                level.BreakBlock(pos, source);
            }
        }
    }
}
