namespace SoulboundEngine.Registry {
	using System.Collections.Generic;

#nullable enable

	public interface IRegistryEntryLookup<T> where T : class {
		RegistryEntry<T>? Get(RegistryKey<T> key);

		RegistryEntry<T> GetOrThrow(RegistryKey<T> key);

		IEnumerable<RegistryKey<T>> GetAllKeys();
	}
}
