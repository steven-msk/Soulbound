namespace SoulboundEngine.Registry {
	using SoulboundEngine.Common;
	using System;
	using System.Collections.Generic;
	using System.Linq;

#nullable enable

	public interface IRegistryManager {
		Registry<T>? Get<T>(RegistryKey<Registry<T>> registryKey) where T : class;

		IEnumerable<IRegistry> GetAll();

		public static IRegistryManager Of(List<IRegistry> registries) {
			return new Impl(registries.ToDictionary(r => r.GetKeyIdentifier()));
		}

		private sealed record Impl(Dictionary<Identifier, IRegistry> registries) : IRegistryManager {
			public Registry<T>? Get<T>(RegistryKey<Registry<T>> registryKey) where T : class {
				return this.registries.GetValueOrDefault(registryKey.value) as Registry<T>;
			}

			public IEnumerable<IRegistry> GetAll() => this.registries.Values;
		}
	}

	public static class RegistryManagerDefaults {
		public static Optional<Registry<T>> GetOptional<T>(this IRegistryManager registryManager, RegistryKey<Registry<T>> registryKey) where T : class {
			return Optional<Registry<T>>.Of(registryManager.Get(registryKey));
		}

		public static Registry<T> GetOrThrow<T>(this IRegistryManager registryManager, RegistryKey<Registry<T>> registryKey) where T : class {
			return registryManager.Get(registryKey) ?? throw new InvalidOperationException("Unknown registry: " + registryKey.value);
		}
	}
}
