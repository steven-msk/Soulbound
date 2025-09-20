using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core.Bootstrap {
    public interface IBootstrappableNodeIndicator {
        public Type Dependency { get; }
    }
}