using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace SoulboundBackend.Client.World.BlockSystem {
    public interface IBlockStateProperty {
        string name { get; }
    }
}
