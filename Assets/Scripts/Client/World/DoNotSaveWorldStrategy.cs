using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable

namespace SoulboundBackend.Client.World {
    public class DoNotSaveWorldStrategy : IWorldSaveStrategy {
		string IWorldSaveStrategy.GetSavesRoot() => Application.temporaryCachePath;

		WorldDump? IWorldSaveStrategy.Load(string world) => null;

		byte[]? IWorldSaveStrategy.LoadRaw(string world) => null;

		void IWorldSaveStrategy.Save(WorldDump obj, string world) {
		}

		void IWorldSaveStrategy.SaveRaw(byte[] data, string world) {
		}
	}
}
