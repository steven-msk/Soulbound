using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.BlockSystem {
    [Obsolete]
    public static class StateCaching {
        public static IBlockStateCacheStrategy Static() {
            return new StaticStateCache();
        }

        public static IPersistentStateCache FileLinked() {
            return new FileLinkedStateCache();
        }

        public static IPersistentStateCache Predefined() {
            return new PredefinedStateCache();
        }
    }
}
