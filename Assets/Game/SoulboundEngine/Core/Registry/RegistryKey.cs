using System;

namespace SoulboundEngine.Core.Registry {
	public sealed class RegistryKey<T> {
		private readonly Identifier registry;
		private readonly Identifier value;

		private RegistryKey(Identifier registry, Identifier value) {
			this.registry = registry;
			this.value = value;
		}

		public Identifier Registry => registry;
		public Identifier Value => value;

		public RegistryKey<Registry<T>> GetRegistryRef() => new(registry, registry);

		public static RegistryKey<T> Of(RegistryKey<Registry<T>> registry, Identifier value) {
			return Of(registry.value, value);
		}

		private static RegistryKey<T> Of(Identifier registry, Identifier value) {
			return new RegistryKey<T>(registry, value);
		}

		public static RegistryKey<Registry<T>> OfRegistry(Identifier registry) {
			return new(Registries.ROOT_IDENTIFIER, registry);
		}

		public override int GetHashCode() => HashCode.Combine(registry, value);

		public override string ToString() {
			return $"registry_key[registry:\"{registry}\", id:\"{value}\"]";
		}
	}
}
