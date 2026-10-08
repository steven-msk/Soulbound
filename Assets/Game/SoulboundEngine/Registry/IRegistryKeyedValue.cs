namespace SoulboundEngine.Registry {
	using System;

	public interface IRegistryKeyedValue<T, V> where T : class {
		V Get(RegistryKey<T> key);

		public static IRegistryKeyedValue<T, V> Of(Func<RegistryKey<T>, V> getter) => new DelegateImpl(getter);

		public static IRegistryKeyedValue<T, V> Fixed(V value) => new FixedImpl(value);

		private sealed record FixedImpl(V value) : IRegistryKeyedValue<T, V> {
			public V Get(RegistryKey<T> key) => this.value;
		}

		private sealed record DelegateImpl(Func<RegistryKey<T>, V> getter) : IRegistryKeyedValue<T, V> {
			public V Get(RegistryKey<T> key) => this.getter(key);
		}
	}
}
