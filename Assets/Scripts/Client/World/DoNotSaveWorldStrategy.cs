using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundBackend.Client.World {
    public class DoNotSaveWorldStrategy : IWorldSaveStrategy {
		string IWorldSaveStrategy.GetSavesRoot() => string.Empty;

		WorldDump? IWorldSaveStrategy.Load(string world) => null;

		byte[]? IWorldSaveStrategy.LoadRaw(string world) => null;

		void IWorldSaveStrategy.Save(WorldDump obj, string world) {
		}

		void IWorldSaveStrategy.SaveRaw(byte[] data, string world) {
		}
	}
}
