namespace SoulboundEngine.Registry {
	using System;

	public interface IRegistryLookup {
		IRegistry Lookup(Identifier id);

		public static IRegistryLookup Of(Func<Identifier, IRegistry> lookup) {
			return new DelegateImpl(lookup);
		}

		private sealed record DelegateImpl(Func<Identifier, IRegistry> lookup) : IRegistryLookup {
			public IRegistry Lookup(Identifier id) {
				return this.lookup(id);
			}
		}
	}

	public static class RegistryLookupDefaults {
		public static Registry<T> Lookup<T>(this IRegistryLookup lookup, RegistryKey<Registry<T>> registryKey) where T : class {
			return (Registry<T>)lookup.Lookup(registryKey.value);
		}
	}
}
