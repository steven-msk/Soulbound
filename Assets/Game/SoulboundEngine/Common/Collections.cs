namespace SoulboundEngine.Common.Collection {
	using System;
	using System.Collections.Generic;

	public static class Collections {
		public static Dictionary<K, V> Dictionary<K, V>() => new();

		public static Dictionary<E, V> Dictionary<E, V>(Func<IEnumerable<E>> keysSupplier, Func<E, V> valueSupplier) {
			Dictionary<E, V> dictionary = new();
			foreach (E key in keysSupplier()) {
				dictionary.Add(key, valueSupplier(key));
			}
			return dictionary;
		} 
	}
}
