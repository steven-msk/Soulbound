using System;

namespace SoulboundEngine.Core.Registry {
	public sealed class RegistryKey<T> {
		public Identifier registry { get; }
		public Identifier value { get; }

		private RegistryKey(Identifier registry, Identifier value) {
			this.registry = registry;
			this.value = value;
		}

		public static RegistryKey<T> Of(RegistryKey<Registry<T>> registry, Identifier value) {
			return Of(registry.value, value);
		}

		private static RegistryKey<T> Of(Identifier registry, Identifier value) {
			return new RegistryKey<T>(registry, value);
		}

		public RegistryKey<Registry<T>> GetRegistryRef() => OfRegistry(registry);

		public static RegistryKey<Registry<T>> OfRegistry(Identifier registry) {
			return new(Registries.ROOT_IDENTIFIER, registry);
		}

		public override int GetHashCode() => HashCode.Combine(registry, value);

		public override string ToString() {
			return $"registry_key[registry:\"{GetRegistryRef()}\", id:\"{value}\"]";
		}
	}
}
