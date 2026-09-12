namespace SoulboundEngine.Registry {
	public record RegistryBootstrapContext(IRegistryLookup lookup) {
		public Registry<T> Lookup<T>(RegistryKey<Registry<T>> registryKey) where T : class {
			return this.lookup.Lookup(registryKey);
		}
	}
}
