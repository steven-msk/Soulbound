using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.BlockSystem {
    [Obsolete]
    public interface IBlockStateCacheResettable {
        public void ResetCache();
    }
}
