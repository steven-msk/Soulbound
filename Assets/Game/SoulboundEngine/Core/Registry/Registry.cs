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

		public static V Register<V>(Registry<T> registry, RegistryKey<T> key, V value) where V : T {
			return (V)RegisterEntry(registry, key, value).GetValue();
		}

		public static V Register<V>(Registry<T> registry, RegistryKey<V> key, V value) where V : T {
			return (V)registry.CreateEntry(key, value).GetValue();
		}

		public static T Register(Registry<T> registry, string id, T value) {
			return RegisterEntry(registry, Identifier.Of(id), value).GetValue();
		}

		public static RegistryEntry<T> RegisterEntry(Registry<T> registry, Identifier id, T value) {
			return RegisterEntry(registry, RegistryKey<T>.Of(registry.key, id), value);
		}

		public static RegistryEntry<T> RegisterEntry(Registry<T> registry, RegistryKey<T> key, T value) {
			return registry.CreateEntry(key, value);
		}

		private RegistryEntry<T> CreateEntry<V>(RegistryKey<V> key, V value) where V : T {
			if (this.freezed) throw new InvalidOperationException("Registry already freezed");

			RegistryKey<T> registryKey = RegistryKey<T>.Of(this.key, key.value);
			RegistryEntry<T> entry = new(this, registryKey, value);

			this.idToEntry.Add(registryKey.value, entry);
			this.keyToEntry.Add(registryKey, entry);
			this.valueToEntry.Add(value, entry);

			return entry;
		}

		public bool Contains(RegistryKey<T> key) => this.keyToEntry.ContainsKey(key);
		public bool ContainsId(Identifier id) => this.idToEntry.ContainsKey(id);

		void IRegistry.Freeze() => this.freezed = true;

		public bool TryGet(RegistryKey<T> key, out T value) {
			RegistryEntry<T>? entry = this.keyToEntry.GetValueOrDefault(key);
			value = entry != null ? entry.GetValue() : default;
			return entry != null;
		}

		public T Get(Identifier id) {
			if (id is null) throw new ArgumentNullException();

			RegistryEntry<T> entry = this.idToEntry.GetValueOrDefault(id) ?? throw new KeyNotFoundException();
			return entry.GetValue();
		}

		public RegistryEntry<T>? GetEntry(Identifier id) => this.idToEntry.GetValueOrDefault(id);

		public RegistryEntry<T>? GetEntry(T value) => this.valueToEntry.GetValueOrDefault(value);

		public RegistryEntry<T>? Get(RegistryKey<T> key) => this.keyToEntry.GetValueOrDefault(key);

		public HashSet<KeyValuePair<RegistryKey<T>, T>> GetEntrySet() {
			return this.keyToEntry
				.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.GetValue())
				.ToHashSet();
		}

		public IEnumerator<T> GetEnumerator() => this.valueToEntry.Keys.GetEnumerator();

		public Identifier? GetIdentifier(T value) {
			if (!this.valueToEntry.TryGetValue(value, out RegistryEntry<T> entry)) {
				return null;
			}
			return entry.GetKey().value;
		}

		public HashSet<Identifier> GetIdentifiers() => this.idToEntry.Keys.ToHashSet();

		public RegistryKey<Registry<T>> GetKey() => this.key;

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

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
