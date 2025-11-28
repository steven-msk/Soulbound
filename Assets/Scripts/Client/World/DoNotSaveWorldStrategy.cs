using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if UNITY_INCLUDE_TESTS
namespace SoulboundBackend.Client.World {
    public class DoNotSaveWorldStrategy : IWorldSaveStrategy {
        public WorldDump? Load(string world) {
            return null;
        }

        public void Save(WorldDump obj, string world) {
        }
    }
}
#endif
