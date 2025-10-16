using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.BlockSystem {
    public static class StateCaching {
        public static IBlockStateCacheStrategy Static() {
            return new StaticStateCache();
        }

        public static IBlockStateCacheStrategy FileLinked() {
            return new FileLinkedStateCache();
        }

        public static IBlockStateCacheStrategy Predefined() {
            return new PredefinedStateCache();
        }
    }
}
