using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core.Bootstrap {
    public interface IBootstrappableNodeHandle {
        public Type Dependency { get; }
    }
}