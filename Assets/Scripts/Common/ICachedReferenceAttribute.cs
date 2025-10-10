using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Common {
    public interface ICachedReferenceAttribute {
        string propertyName { get; }
    }
}
