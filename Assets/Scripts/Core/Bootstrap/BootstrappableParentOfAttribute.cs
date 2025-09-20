using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core.Bootstrap {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class BootstrappableParentOfAttribute : Attribute, IBootstrappableNodeIndicator {
        public Type Dependency { get; }

        public BootstrappableParentOfAttribute(Type dependency) {
            Dependency = dependency;
        }
    }
}
