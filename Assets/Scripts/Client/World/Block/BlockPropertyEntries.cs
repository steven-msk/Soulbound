using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.BlockSystem {
	public sealed class BlockPropertyEntries {
		private readonly Dictionary<IBlockStateProperty, object> entries = new();
		private readonly BlockPropertyPool pool;

		public BlockPropertyEntries(BlockPropertyPool pool) {
			this.pool = pool;
		}

		public T Get<T>(BlockProperty<T> property) {
			if (!entries.ContainsKey(property)) {
				return (T)GetZeroValue(property);
			}
			return (T)entries[property];
		}

		public BlockPropertyEntries With<T>(BlockProperty<T> property, T value) {
			entries[property] = value;
			return this;
		}

		private object GetZeroValue(IBlockStateProperty prop) {
			var type = prop.valueType;
			return type.IsValueType ? Activator.CreateInstance(type)! : null!;
		}

		public override bool Equals(object obj) {
			return obj is BlockPropertyEntries other
				&& other.pool == this.pool
				&& other.entries.SequenceEqual(this.entries);
		}

		public override int GetHashCode() {
			return HashCode.Combine(entries, pool);
		}
	}
}
