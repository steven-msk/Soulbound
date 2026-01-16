using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundBackend.Client.World {
    public class DoNotSaveWorldStrategy : IWorldSaveStrategy {
        public WorldDump? Load(string world) {
            return null;
        }

		public byte[]? LoadRaw(string world) {
			return null;
		}

		public void Save(WorldDump obj, string world) {
        }

		public void SaveRaw(byte[] data, string world) {
		}
	}
}
