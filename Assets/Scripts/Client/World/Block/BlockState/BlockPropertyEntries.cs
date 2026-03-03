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

		private BlockPropertyEntries(BlockPropertyPool pool, Dictionary<IBlockStateProperty, object> entries) {
			this.pool = pool;
			this.entries = entries;
		}

		public T Get<T>(BlockProperty<T> property) {
			if (!entries.ContainsKey(property)) return (T)GetZeroValue(property);
			return (T)entries[property];
		}

		public BlockPropertyEntries With<T>(BlockProperty<T> property, T value) {
			Dictionary<IBlockStateProperty, object> newEntries = new(entries) {
				[property] = value
			};
			return new BlockPropertyEntries(pool, newEntries);
		}

		public BlockPropertyEntries With<T>(string property, T value) {
			return With(pool.Get<T>(property), value);
		}

		private object GetZeroValue(IBlockStateProperty prop) {
			var type = prop.valueType;
			return type.IsValueType
				? Activator.CreateInstance(type)
				: null;
		}

		public override bool Equals(object obj) {
			return obj is BlockPropertyEntries other
				&& other.pool == this.pool
				&& other.entries.SequenceEqual(this.entries);
		}

		public override string ToString() {
			return string.Join(',', entries.Select(kvp => $"{kvp.Key}={kvp.Value}"));
		}

		public override int GetHashCode() {
			return HashCode.Combine(entries, pool);
		}

		public IDictionary<string, object> GetSorted() {
			return entries
				.OrderBy(kvp => kvp.Key.name)
				.ToDictionary(kvp => kvp.Key.name, kvp => kvp.Value);
		}
	}
}
