using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if UNITY_INCLUDE_TESTS
namespace SoulboundBackend.Client.World {
    public class DoNotSaveWorldStrategy : ISaveStrategy<WorldDump> {
        public WorldDump Load(string path) {
            return default;
        }

        public void Save(WorldDump obj, string path) {
        }
    }
}
#endif
