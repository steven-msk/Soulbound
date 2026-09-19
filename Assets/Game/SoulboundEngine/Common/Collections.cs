namespace SoulboundEngine.Common.Collection {
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public static class Collections {
		public static Dictionary<K, V> Dictionary<K, V>() => new();

		public static Dictionary<E, V> Dictionary<E, V>(Func<IEnumerable<E>> keysSupplier, Func<E, V> valueSupplier) {
			Dictionary<E, V> dictionary = new();
			foreach (E key in keysSupplier()) {
				dictionary.Add(key, valueSupplier(key));
			}
			return dictionary;
		}

		public static Dictionary<K, V> MapFromKey<K, E, V>(IDictionary<K, E> first, IDictionary<E, V> second) {
			Dictionary<K, V> result = new();
			foreach ((K key, E element) in first) {
				if (second.TryGetValue(element, out V value)) {
					result.Add(key, value);
				}
			}
			return result;
		}

		public static IEnumerable<T> Single<T>(T value) => Enumerable.Repeat(value, 1);

		public static void ForEach<T>(this IEnumerable<T> values, Action<T> action) {
			foreach (T value in values) {
				action(value);
			}
		}

		public static void ForEach<K, V>(this IDictionary<K, V> dictionary, Action<K, V> action) {
			foreach ((K key, V value) in dictionary) {
				action(key, value);
			}
		}

		public static V GetOrThrow<K, V>(this IDictionary<K, V> dictionary, K key) {
			return dictionary[key];
		}

		public static V AddIfAbsent<K, V>(this IDictionary<K, V> dictionary, K key, Func<K, V> valueSupplier) {
			if (dictionary.TryGetValue(key, out V value)) {
				return value;
			}
			value = valueSupplier(key);
			dictionary.Add(key, value);
			return value;
		}

		public static V Put<K, V>(this IDictionary<K, V> dictionary, K key, V value) {
			dictionary.Add(key, value);
			return value;
		}

		public static VDerived Put<K, V, VDerived>(this IDictionary<K, V> dictionary, K key, VDerived derivedValue) where VDerived : V {
			dictionary.Add(key, derivedValue);
			return derivedValue;
		}

		public static V Set<K, V>(this IDictionary<K, V> dictionary, K key, V value) {
			return dictionary[key] = value;
		}
	}
}
