#nullable enable

using System.Collections.Generic;

namespace SoulboundEngine.Core.Registry {
	public interface IRegistryEntryLookup<T> {
		RegistryEntry<T>? Get(RegistryKey<T> key);

		public RegistryEntry<T> GetOrThrow(RegistryKey<T> key) {
			return Get(key) ?? throw new KeyNotFoundException($"Entry not found: {key}");
		}

		public interface IRegistryLookup {
			IRegistryEntryLookup<T>? Get<TRegistry>(RegistryKey<TRegistry> registryRef) where TRegistry : IRegistry;

			public IRegistryEntryLookup<T> GetOrThrow<TRegistry>(RegistryKey<TRegistry> registryRef) where TRegistry : IRegistry {
				return Get(registryRef) ?? throw new KeyNotFoundException($"Registry not found: {registryRef}");
			}
		}
	}
}
