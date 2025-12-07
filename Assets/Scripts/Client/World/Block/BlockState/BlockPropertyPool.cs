using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.BlockSystem {
	public sealed class BlockPropertyPool {
		private readonly Dictionary<string, IBlockStateProperty> pool = new();
		public IEnumerable<IBlockStateProperty> AllProperties => pool.Values;

		public BlockProperty<T> Register<T>(string name) {
			var property = new BlockProperty<T>(name);
			pool[name] = property;
			return property;
		}

		public bool Has(string property) {
			return pool.ContainsKey(property);
		}

		public BlockProperty<T> Get<T>(string property) {
			return (BlockProperty<T>)pool[property];
		}

		public override bool Equals(object obj) {
			return obj is BlockPropertyPool other
				&& other.pool.SequenceEqual(this.pool);
		}

		public override int GetHashCode() {
			return pool.GetHashCode();
		}

		public BlockPropertyEntries CreateEntries() => new(this);

		public override string ToString() {
			return string.Join(',', pool.Keys);
		}
	}
}
