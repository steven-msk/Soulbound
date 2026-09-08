namespace SoulboundEngine.Registry {
	using SoulboundEngine.Serialization;
	using System;

	public sealed class RegistryKey<T> where T : class {
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

		public static RegistryKey<Registry<T>> OfRegistry(Identifier registry) {
			return new(Registries.ROOT_IDENTIFIER, registry);
		}

		public static Codec<RegistryKey<T>> Codec(RegistryKey<Registry<T>> registryKey) {
			return Identifier.CODEC.Xmap(i => Of(registryKey, i), k => k.value);
		}

		public RegistryKey<Registry<T>> GetRegistryRef() => OfRegistry(this.registry);

		public override int GetHashCode() => HashCode.Combine(this.registry, this.value);

		public override string ToString() {
			return $"RegistryKey[{this.registry}/{this.value}]";
		}
	}
}
