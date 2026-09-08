namespace SoulboundEngine.Registry {
	using System.Collections.Generic;

#nullable enable

	public interface IRegistryEntryLookup<T> where T : class {
		RegistryEntry<T>? Get(RegistryKey<T> key);

		public RegistryEntry<T> GetOrThrow(RegistryKey<T> key) {
			return this.Get(key) ?? throw new KeyNotFoundException($"Entry not found: {key}");
		}

		IEnumerable<RegistryKey<T>> GetAllKeys();

		public interface IRegistryLookup {
			IRegistryEntryLookup<T>? Get<TRegistry>(RegistryKey<TRegistry> registryRef) where TRegistry : class, IRegistry;

			public IRegistryEntryLookup<T> GetOrThrow<TRegistry>(RegistryKey<TRegistry> registryRef) where TRegistry : class, IRegistry {
				return this.Get(registryRef) ?? throw new KeyNotFoundException($"Registry not found: {registryRef}");
			}
		}
	}
}
