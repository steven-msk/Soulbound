using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace SoulboundEngine.Core.Registry {
	public sealed class Registry<T> : IRegistry, IRegistryEntryOwner<T>, IRegistryEntryLookup<T>, IEnumerable<T> {
		private readonly Dictionary<Identifier, RegistryEntry<T>> idToEntry = new();
		private readonly Dictionary<RegistryKey<T>, RegistryEntry<T>> keyToEntry = new(new KeyComparer());
		private readonly Dictionary<T, RegistryEntry<T>> valueToEntry = new();
		private readonly RegistryKey<Registry<T>> key;
		private bool freezed = false;

		public Registry(RegistryKey<Registry<T>> key) => this.key = key;

		public RegistryEntry<T> CreateEntry(Identifier id, T value) {
			if (freezed) throw new InvalidOperationException("Registry already freezed");

			RegistryKey<T> key = RegistryKey<T>.Of(this.key, id);
			RegistryEntry<T> entry = new(this, key, value);

			idToEntry.Add(id, entry);
			keyToEntry.Add(key, entry);
			valueToEntry.Add(value, entry);

			return entry;
		}

		public bool Contains(RegistryKey<T> key) => keyToEntry.ContainsKey(key);
		public bool ContainsId(Identifier id) => idToEntry.ContainsKey(id);

		void IRegistry.Freeze() => freezed = true;

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

		public RegistryEntry<T>? Get(RegistryKey<T> key) => keyToEntry.GetValueOrDefault(key);

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
			return entry.GetKey().value;
		}

		public HashSet<Identifier> GetIdentifiers() => idToEntry.Keys.ToHashSet();

		public RegistryKey<Registry<T>> GetKey() => key;

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		bool IRegistryEntryOwner<T>.OwnerEquals(IRegistryEntryOwner<T> other) {
			return this == other;
		}

		private sealed class KeyComparer : IEqualityComparer<RegistryKey<T>> {
			public bool Equals(RegistryKey<T> x, RegistryKey<T> y) {
				return x.value.Equals(y.value) && x.registry.Equals(y.registry);
			}

			public int GetHashCode(RegistryKey<T> obj) => obj.GetHashCode();
		}

	}
}
