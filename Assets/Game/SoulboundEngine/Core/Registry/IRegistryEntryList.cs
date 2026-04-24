using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SoulboundEngine.Core.Registry {
	public interface IRegistryEntryList<T> : IEnumerable<RegistryEntry<T>> {
		int size { get; }
		bool Contains(RegistryEntry<T> entry);

		public abstract class ListBacked : IRegistryEntryList<T> {
			protected abstract List<RegistryEntry<T>> entries { get; }

			public abstract bool Contains(RegistryEntry<T> entry);

			public int size => entries.Count;

			public RegistryEntry<T> Get(int index) => entries[index];

			public IEnumerator<RegistryEntry<T>> GetEnumerator() => entries.GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}

		public sealed class Direct : ListBacked {
			internal static readonly Direct EMPTY;
			protected override List<RegistryEntry<T>> entries { get; }
			private readonly HashSet<RegistryEntry<T>> entrySet;

			public Direct(List<RegistryEntry<T>> entries) {
				this.entries = entries;
				this.entrySet = entries.ToHashSet();
			}

			public override bool Contains(RegistryEntry<T> entry) {
				return entrySet.Contains(entry);
			}

			public override int GetHashCode() => HashCode.Combine(entries);

			public override string ToString() {
				return $"direct[{string.Join(", ", entries)}]";
			}
		}
	}
}
