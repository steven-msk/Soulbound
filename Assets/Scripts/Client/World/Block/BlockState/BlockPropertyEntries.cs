using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.BlockSystem {
	public sealed class BlockPropertyEntries {
		// note that invalid property states are possible for this configuration
		private readonly Dictionary<string, object> entries = new();

		public BlockPropertyEntries() { }

		private BlockPropertyEntries(Dictionary<string, object> entries) {
			this.entries = entries;
		}

		public BlockPropertyEntries With<T>(string property, T value) {
			Dictionary<string, object> newEntries = new(entries) {
				[property] = value
			};
			return new BlockPropertyEntries(newEntries);
		}

		public T Get<T>(string property) => (T)entries[property];

		public bool TryGet<T>(string property, out T value) {
			if (entries.TryGetValue(property, out object boxed)) {
				value = (T)boxed;
				return true;
			}
			value = default;
			return false;
		}

		public override bool Equals(object obj) {
			return obj is BlockPropertyEntries other
				&& other.entries.SequenceEqual(this.entries);
		}

		public override string ToString() {
			return string.Join(',', entries.Select(kvp => $"{kvp.Key}={kvp.Value}"));
		}

		public override int GetHashCode() {
			return HashCode.Combine(entries);
		}

		public IDictionary<string, object> GetSorted() {
			return entries
				.OrderBy(kvp => kvp.Key)
				.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
		}
	}
}
