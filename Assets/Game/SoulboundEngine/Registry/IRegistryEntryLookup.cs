namespace SoulboundEngine.Registry {
	using System.Collections.Generic;

#nullable enable

	public interface IRegistryEntryLookup<T> where T : class {
		RegistryEntry<T>? Get(RegistryKey<T> key);

		public RegistryEntry<T> GetOrThrow(RegistryKey<T> key) {
			return this.Get(key) ?? throw new KeyNotFoundException($"Entry not found: {key}");
		}

		IEnumerable<RegistryKey<T>> GetAllKeys();
	}
}
