#nullable enable

using System.Collections.Generic;

namespace SoulboundEngine.Registry {
	public interface IRegistryEntryLookup<T> {
		RegistryEntry<T>? Get(RegistryKey<T> key);

		public RegistryEntry<T> GetOrThrow(RegistryKey<T> key) {
			return this.Get(key) ?? throw new KeyNotFoundException($"Entry not found: {key}");
		}

		IEnumerable<RegistryKey<T>> GetAllKeys();

		public interface IRegistryLookup {
			IRegistryEntryLookup<T>? Get<TRegistry>(RegistryKey<TRegistry> registryRef) where TRegistry : IRegistry;

			public IRegistryEntryLookup<T> GetOrThrow<TRegistry>(RegistryKey<TRegistry> registryRef) where TRegistry : IRegistry {
				return this.Get(registryRef) ?? throw new KeyNotFoundException($"Registry not found: {registryRef}");
			}
		}
	}
}
