using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.BlockSystem {
    public class BlockProperty<T> : IBlockStateProperty {
        public string name { get; }
        public IReadOnlyList<T> allowedValues { get; }

        public BlockProperty(string name, params T[] allowedValues) {
            this.name = name;
            this.allowedValues = allowedValues;
        }

        public override string ToString() => name;

        public override bool Equals(object obj) {
            return obj is BlockProperty<T> other
                && other.name == this.name
                && other.allowedValues.SequenceEqual(this.allowedValues);
        }

        public override int GetHashCode() {
            return HashCode.Combine(name, allowedValues);
        }
    }
}
