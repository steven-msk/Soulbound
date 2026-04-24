using System;

namespace SoulboundEngine.Core.Registry {
	public class RegistryEntry<T> {
		private readonly IRegistryEntryOwner<T> owner;
		private readonly RegistryKey<T> key;
		private readonly T value;

		public RegistryEntry(IRegistryEntryOwner<T> owner, RegistryKey<T> key, T value) {
			this.owner = owner;
			this.key = key;
			this.value = value;
		}

		public RegistryKey<T> GetKey() => key;
		public T GetValue() => value;

		public string GetIdAsString() => GetKey()?.value.ToString() ?? "null";

		public bool MatchesId(Identifier id) {
			return key.value.Equals(id);
		}
		public bool MatchesKey(RegistryKey<T> key) {
			return this.key.value.Equals(key.value) && this.key.registry.Equals(key.registry);
		}

		public override int GetHashCode() => HashCode.Combine(owner, key, value);
	}
}
