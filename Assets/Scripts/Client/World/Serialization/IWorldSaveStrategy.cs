using SoulboundBackend.Client.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundBackend.Core {
    public interface IWorldSaveStrategy {
		string GetSavesRoot();

        WorldDump? Load(string world);
        void Save(WorldDump obj, string world);

        byte[]? LoadRaw(string world);
        void SaveRaw(byte[] data, string world);
    }
}
