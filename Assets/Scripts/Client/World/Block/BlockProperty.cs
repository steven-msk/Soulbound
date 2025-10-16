using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.BlockSystem {
	public class BlockProperty<T> : IBlockStateProperty {
		public string name { get; }

		public BlockProperty(string name) {
			this.name = name;
		}

		public override string ToString() => name;

		public override bool Equals(object obj) {
			return obj is BlockProperty<T> other
				&& other.name == this.name;
		}

		public override int GetHashCode() {
			return HashCode.Combine(name, typeof(T));
		}
	}
}
