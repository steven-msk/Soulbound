using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundEngine.Tests {
    public class ContextBox<T> {
        public T? value { get; set; } = default(T);
    }
}
