using SoulboundBackend.Client.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundBackend.Core {
    public interface ISaveStrategy<T> {
        T? Load(string path);
        void Save(T obj, string path);
    }
}
