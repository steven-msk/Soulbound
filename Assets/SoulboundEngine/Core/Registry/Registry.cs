using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace SoulboundEngine.Core.Registry {
	public sealed class Registry<T> : IRegistry, IRegistryEntryOwner<T>, IEnumerable<T> {
		private readonly Dictionary<Identifier, RegistryEntry<T>> idToEntry = new();
		private readonly Dictionary<RegistryKey<T>, RegistryEntry<T>> keyToEntry = new(new KeyComparer());
		private readonly Dictionary<T, RegistryEntry<T>> valueToEntry = new();
		private readonly RegistryKey<Registry<T>> key;

		public Registry(RegistryKey<Registry<T>> key) => this.key = key;

		public RegistryEntry<T> CreateEntry(Identifier id, T value) {
			RegistryKey<T> key = RegistryKey<T>.Of(this.key, id);
			RegistryEntry<T> entry = new(this, key, value);

			idToEntry.Add(id, entry);
			keyToEntry.Add(key, entry);
			valueToEntry.Add(value, entry);

			return entry;
		}

		public bool Contains(RegistryKey<T> key) => keyToEntry.ContainsKey(key);
		public bool ContainsId(Identifier id) => idToEntry.ContainsKey(id);

		public T Get(RegistryKey<T> key) {
			if (key is null) throw new ArgumentNullException();

			RegistryEntry<T> entry = keyToEntry.GetValueOrDefault(key) ?? throw new KeyNotFoundException();
			return entry.GetValue();
		}

		public bool TryGet(RegistryKey<T> key, out T value) {
			RegistryEntry<T>? entry = keyToEntry.GetValueOrDefault(key);
			value = entry != null ? entry.GetValue() : default;
			return entry != null;
		}

		public T Get(Identifier id) {
			if (id is null) throw new ArgumentNullException();

			RegistryEntry<T> entry = idToEntry.GetValueOrDefault(id) ?? throw new KeyNotFoundException();
			return entry.GetValue();
		}

		public RegistryEntry<T>? GetEntry(Identifier id) => idToEntry.GetValueOrDefault(id);

		public RegistryEntry<T>? GetEntry(T value) => valueToEntry.GetValueOrDefault(value);

		public HashSet<KeyValuePair<RegistryKey<T>, T>> GetEntrySet() {
			return keyToEntry
				.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.GetValue())
				.ToHashSet();
		}

		public IEnumerator<T> GetEnumerator() => valueToEntry.Keys.GetEnumerator();

		public Identifier? GetIdentifier(T value) {
			if (!valueToEntry.TryGetValue(value, out RegistryEntry<T> entry)) {
				return null;
			}
			return entry.GetKey().Value;
		}

		public HashSet<Identifier> GetIdentifiers() => idToEntry.Keys.ToHashSet();

		public RegistryKey<Registry<T>> GetKey() => key;

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		private sealed class KeyComparer : IEqualityComparer<RegistryKey<T>> {
			public bool Equals(RegistryKey<T> x, RegistryKey<T> y) {
				return x.Value.Equals(y.Value) && x.Registry.Equals(y.Registry);
			}

			public int GetHashCode(RegistryKey<T> obj) => obj.GetHashCode();
		}
	}
}
