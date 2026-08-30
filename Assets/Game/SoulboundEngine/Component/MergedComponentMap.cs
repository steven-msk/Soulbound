namespace SoulboundEngine.Component {
	using SoulboundEngine.Common.Collection;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;

	public sealed class MergedComponentMap : IComponentMap {
		public static readonly MergedComponentMap EMPTY = Create(IComponentMap.EMPTY, ComponentChanges.EMPTY);
		private readonly IComponentMap baseMap;
		private Dictionary<ComponentType, object> changedComponents;

		public MergedComponentMap(IComponentMap baseMap)
			: this(baseMap, Collections.Dictionary<ComponentType, object>()) {
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

			foreach ((ComponentType type, object value) in this.changedComponents) {
				if (ReferenceEquals(value, Component.REMOVED)) {
					builder.Remove(type);
				} else {
					builder.AddRaw(type, value);
				}
			}
			return builder.Build();
		}

		public T Get<T>(ComponentType<T> type) {
			return this.changedComponents.TryGetValue(type, out object value)
				? ReferenceEquals(value, Component.REMOVED)
					? throw new KeyNotFoundException($"Component {type} was removed from this patch.")
					: (T)value
				: this.baseMap.Get(type);
		}

		public T GetOrDefault<T>(ComponentType<T> type, T fallback) {
			return this.changedComponents.TryGetValue(type, out object value)
				? ReferenceEquals(value, Component.REMOVED) ? fallback : (T)value
				: this.baseMap.GetOrDefault(type, fallback);
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
			foreach (Component component in components) {
				ComponentType type = component.boxedType;
				object value = component.boxedValue;
				this.changedComponents[type] = value;
			}
		}

		public ISet<ComponentType> GetTypes() {
			HashSet<ComponentType> types = new(this.baseMap.GetTypes());

			foreach ((ComponentType type, object value) in this.changedComponents) {
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

			foreach (Component component in this.baseMap) {
				merged[component.boxedType] = component.boxedValue;
			}
			foreach ((ComponentType type, object value) in this.changedComponents) {
				if (ReferenceEquals(value, Component.REMOVED)) {
					merged.Remove(type);
				} else {
					merged[type] = value;
				}
			}
			foreach (KeyValuePair<ComponentType, object> kvp in merged) {
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
			return obj is MergedComponentMap other && this.baseMap.Equals(other.baseMap)
				&& this.changedComponents.Count == other.changedComponents.Count
				&& this.changedComponents.All(kvp =>
					other.changedComponents.TryGetValue(kvp.Key, out object v)
					&& (ReferenceEquals(kvp.Value, Component.REMOVED) ? ReferenceEquals(v, Component.REMOVED) : kvp.Value.Equals(v)));
		}

		public override int GetHashCode() {
			int hash = this.baseMap.GetHashCode();
			foreach (KeyValuePair<ComponentType, object> kvp in this.changedComponents.OrderBy(k => k.Key.GetHashCode())) {
				hash = HashCode.Combine(hash, kvp.Key, ReferenceEquals(kvp.Value, Component.REMOVED) ? 0 : kvp.Value.GetHashCode());
			}
			return hash;
		}
	}
}
