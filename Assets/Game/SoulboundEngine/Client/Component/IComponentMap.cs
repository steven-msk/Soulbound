using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SoulboundEngine.Client.Component {
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

			public override int GetHashCode() {
				return HashCode.Combine(this.map);
			}

			public override bool Equals(object obj) {
				return obj is DictionaryBackedComponentMap other && other.map.Equals(this.map);
			}
		}

		public class Builder {
			private readonly Dictionary<ComponentType, object> map = new();

			public IComponentMap Build() => Build(this.map);

			private static IComponentMap Build(Dictionary<ComponentType, object> map) {
				return new DictionaryBackedComponentMap(map);
			}

			public Builder Add<T>(ComponentType<T> type, T value) {
				this.map.Add(type, value);
				return this;
			}
			

			public Builder AddAll(IComponentMap map) {
				foreach (var component in map) {
					this.Add(component);
				}
				return this;
			}

			internal void Add(Component component) {
				this.map.Add(component.boxedType, component.boxedValue);
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
