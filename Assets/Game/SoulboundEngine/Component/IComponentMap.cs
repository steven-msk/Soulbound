namespace SoulboundEngine.Component {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;

	public interface IComponentMap : IEnumerable<Component>, IComponentsAccess {
		public static IComponentMap EMPTY = Create().Build();

		ISet<ComponentType> GetTypes();

		public static Builder Create() => new();

		private sealed class DictionaryBackedComponentMap : IComponentMap {
			private readonly Dictionary<ComponentType, object> map = new();

			public DictionaryBackedComponentMap(Dictionary<ComponentType, object> map) {
				this.map = map;
			}

			public T Get<T>(ComponentType<T> type) => (T)this.map[type];

			public T GetOrDefault<T>(ComponentType<T> type, T fallback) {
				return (T)this.map.GetValueOrDefault(type, fallback);
			}

			public IEnumerator<Component> GetEnumerator() {
				return this.map.Select(kvp => Component.Of(kvp.Key, kvp.Value)).GetEnumerator();
			}

			public ISet<ComponentType> GetTypes() => this.map.Keys.ToHashSet();

			IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

			public override bool Equals(object obj) {
				if (obj is not DictionaryBackedComponentMap other) return false;
				if (this.map.Count != other.map.Count) return false;
				foreach ((ComponentType key, object value) in this.map) {
					if (!other.map.TryGetValue(key, out var otherValue) || !Equals(value, otherValue)) {
						return false;
					}
				}
				return true;
			}

			public override int GetHashCode() {
				int hash = 0;
				foreach (KeyValuePair<ComponentType, object> kvp in this.map.OrderBy(k => k.Key.GetHashCode())) {
					hash = HashCode.Combine(hash, kvp.Key, kvp.Value);
				}
				return hash;
			}
		}

		public class Builder {
			private readonly Dictionary<ComponentType, object> map = new();

			public IComponentMap Build() => Build(this.map);

			private static IComponentMap Build(Dictionary<ComponentType, object> map) {
				return new DictionaryBackedComponentMap(new Dictionary<ComponentType, object>(map));
			}

			public Builder Add<T>(ComponentType<T> type, T value) {
				this.map[type] = value;
				return this;
			}
			

			public Builder AddAll(IComponentMap map) {
				foreach (Component component in map) {
					this.Add(component);
				}
				return this;
			}

			internal void Add(Component component) {
				this.map[component.boxedType] = component.boxedValue;
			}
		}
	}

	public static class ComponentMapExtensions {
		public static bool Contains(this IComponentMap map, ComponentType type) {
			return map.GetTypes().Contains(type);
		}

		public static bool IsEmpty(this IComponentMap map) {
			return map.GetTypes().Count == 0;
		}

		public static int Size(this IComponentMap map) {
			return map.GetTypes().Count;
		}
	}
}
