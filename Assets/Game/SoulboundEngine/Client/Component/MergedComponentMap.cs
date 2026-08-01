using SoulboundEngine.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SoulboundEngine.Client.Component {
	public sealed class MergedComponentMap : IComponentMap {
		private readonly IComponentMap baseMap;
		private Dictionary<ComponentType, object> changedComponents;

		public MergedComponentMap(IComponentMap baseMap)
			: this(baseMap, Dictionaries.Empty<ComponentType, object>()) {
		}

		private MergedComponentMap(IComponentMap baseMap, Dictionary<ComponentType, object> changedComponents) {
			this.baseMap = baseMap;
			this.changedComponents = changedComponents;
		}

		public static MergedComponentMap Create(IComponentMap baseMap, ComponentChanges changes) {
			MergedComponentMap map = new(baseMap);
			map.SetChanges(changes);
			return map;
		}

		public void SetChanges(ComponentChanges changes) {
			this.changedComponents = new Dictionary<ComponentType, object>(changes.GetChanges());
		}

		public ComponentChanges AsPatch() {
			ComponentChanges.Builder builder = ComponentChanges.Create();

			foreach (var (type, value) in this.changedComponents) {
				if (ReferenceEquals(value, Component.REMOVED)) {
					builder.Remove(type);
				} else {
					builder.AddRaw(type, value);
				}
			}
			return builder.Build();
		}

		public T Get<T>(ComponentType<T> type) {
			if (this.changedComponents.TryGetValue(type, out object value)) {
				if (ReferenceEquals(value, Component.REMOVED)) {
					throw new KeyNotFoundException($"Component {type} was removed from this patch.");
				}
				return (T)value;
			}
			return this.baseMap.Get(type);
		}

		public T GetOrDefault<T>(ComponentType<T> type, T fallback) {
			if (this.changedComponents.TryGetValue(type, out object value)) {
				return ReferenceEquals(value, Component.REMOVED) ? fallback : (T)value;
			}
			return this.baseMap.GetOrDefault(type, fallback);
		}

		public void Set<T>(ComponentType<T> type, T value) {
			this.changedComponents[type] = value;
		}

		public T Remove<T>(ComponentType<T> type) {
			T old = this.GetOrDefault(type, default);
			this.changedComponents[type] = Component.REMOVED;
			return old;
		}

		public void ClearChanges() {
			this.changedComponents.Clear();
		}

		public bool HasAnyChanges() => this.changedComponents.Count == 0;

		public bool HasChanged(ComponentType type) {
			return this.changedComponents.ContainsKey(type);
		}

		public void SetAll(IComponentMap components) {
			foreach (var component in components) {
				ComponentType type = component.boxedType;
				object value = component.boxedValue;
				this.changedComponents[type] = value;
			}
		}

		public ISet<ComponentType> GetTypes() {
			HashSet<ComponentType> types = new(this.baseMap.GetTypes());

			foreach (var (type, value) in this.changedComponents) {
				if (ReferenceEquals(value, Component.REMOVED)) {
					types.Remove(type);
				} else {
					types.Add(type);
				}
			}
			return types;
		}

		public IEnumerator<Component> GetEnumerator() {
			Dictionary<ComponentType, object> merged = new();

			foreach (var component in this.baseMap) {
				merged[component.boxedType] = component.boxedValue;
			}
			foreach (var (type, value) in this.changedComponents) {
				if (ReferenceEquals(value, Component.REMOVED)) {
					merged.Remove(type);
				} else {
					merged[type] = value;
				}
			}
			foreach (var kvp in merged) {
				yield return Component.Of(kvp.Key, kvp.Value);
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		public IComponentMap ToImmutable() {
			IComponentMap.Builder builder = IComponentMap.Create();
			builder.AddAll(this);
			return builder.Build();
		}

		public MergedComponentMap Copy() {
			return new MergedComponentMap(this.baseMap, new Dictionary<ComponentType, object>(this.changedComponents));
		}

		public override bool Equals(object obj) {
			if (obj is not MergedComponentMap other) return false;
			return this.baseMap.Equals(other.baseMap)
				&& this.changedComponents.Count == other.changedComponents.Count
				&& this.changedComponents.All(kvp =>
					other.changedComponents.TryGetValue(kvp.Key, out var v)
					&& (ReferenceEquals(kvp.Value, Component.REMOVED) ? ReferenceEquals(v, Component.REMOVED) : kvp.Value.Equals(v)));
		}

		public override int GetHashCode() {
			int hash = this.baseMap.GetHashCode();
			foreach (var kvp in this.changedComponents.OrderBy(k => k.Key.GetHashCode())) {
				hash = HashCode.Combine(hash, kvp.Key, ReferenceEquals(kvp.Value, Component.REMOVED) ? 0 : kvp.Value.GetHashCode());
			}
			return hash;
		}
	}
}
