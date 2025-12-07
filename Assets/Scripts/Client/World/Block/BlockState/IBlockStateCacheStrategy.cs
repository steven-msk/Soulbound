using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.BlockSystem {
    [Obsolete]
    public interface IBlockStateCacheStrategy {
        void Initialize(Block block);
        void RegisterDefault(BlockState defaultState);
        BlockState Get(Block block, BlockStateProperties properties);
        BlockState Get(Block block, int hash);
    }
}
