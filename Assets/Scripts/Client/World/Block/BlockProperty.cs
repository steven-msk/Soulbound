using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.BlockSystem {
    public class BlockProperty<T> {
        public string name { get; }
        public IReadOnlyList<T> allowedValues { get; }

        public BlockProperty(string name, params T[] allowedValues) {
            this.name = name;
            this.allowedValues = allowedValues;
        }

        public override string ToString() => name;
    }
}
