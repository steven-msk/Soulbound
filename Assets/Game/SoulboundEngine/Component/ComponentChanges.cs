using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Registry;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace SoulboundEngine.Component {
	public sealed class ComponentChanges {
		private readonly Dictionary<ComponentType, object> changedComponents;
		public static readonly ComponentChanges EMPTY = new(new Dictionary<ComponentType, object>());

		private ComponentChanges(Dictionary<ComponentType, object> changedComponents) {
			this.changedComponents = changedComponents;
		}

		public static Builder Create() => new();

		public IReadOnlyDictionary<ComponentType, object> GetChanges() => this.changedComponents;

		public bool IsEmpty() => this.changedComponents.Count == 0;

		public int Size() => this.changedComponents.Count;

		public ISet<KeyValuePair<ComponentType, object>> EntrySet() {
			return this.changedComponents.ToHashSet();
		}

		public T? Get<T>(ComponentType<T> type) => (T)this.changedComponents.GetValueOrDefault(type);

		public static ComponentChanges Compare(IComponentMap baseMap, IComponentMap result) {
			Dictionary<ComponentType, object> baseValues = baseMap.ToDictionary(c => c.boxedType, c => c.boxedValue);
			Builder builder = Create();
			HashSet<ComponentType> seen = new();

			foreach (Component? component in result) {
				seen.Add(component.boxedType);
				if (!baseValues.TryGetValue(component.boxedType, out var baseValue)
					|| !baseValue.Equals(component.boxedValue)) {
					builder.AddRaw(component.boxedType, component.boxedValue);
				}
			}
			foreach (ComponentType type in baseValues.Keys) {
				if (!seen.Contains(type)) {
					builder.Remove(type);
				}
			}
			return builder.Build();
		}

		public ComponentChanges WithRemovedIf(Predicate<ComponentType> removedTypePredicate) {
			Builder builder = Create();
			foreach (ComponentType type in this.changedComponents.Keys) {
				if (removedTypePredicate(type)) {
					builder.Remove(type);
				} else {
					builder.AddRaw(type, this.changedComponents[type]);
				}
			}
			return builder.Build();
		}

		public AddedRemovedPairs AsAddedRemoved() {
			Dictionary<ComponentType, object> addedComponents = new();
			HashSet<ComponentType> removed = new();

			foreach ((ComponentType type, object value) in this.changedComponents) {
				if (Component.IsRemoved(value)) {
					removed.Add(type);
				} else {
					addedComponents[type] = value;
				}
			}

			IComponentMap.Builder builder = IComponentMap.Create();
			foreach (var kvp in addedComponents) {
				builder.Add(Component.Of(kvp.Key, kvp.Value));
			}

			return new AddedRemovedPairs(builder.Build(), removed);
		}

		public static JToken ToJson(ComponentChanges changes) {
			if (changes.Equals(EMPTY)) return JValue.CreateNull();

			JObject json = new();
			foreach ((ComponentType type, object value) in changes.changedComponents) {
				Identifier id = ComponentType.GetId(type);
				json.Add(id.ToString(), Component.IsRemoved(value) ? null : type.ToJson(value));
			}
			return json;
		}

		public static ComponentChanges FromJson(JObject json) {
			if (json.Type == JTokenType.Null) return EMPTY;
			if (json.Type != JTokenType.Object) {
				throw new InvalidOperationException("ComponentChanges json is not object");
			}

			Builder builder = Create();
			foreach (JProperty property in json.Properties()) {
				Identifier typeId = Identifier.Of(property.Name);
				ComponentType? type = ComponentType.Get(typeId);
				if (type == null) {
					Logger.LogError("Skipping unknown component type {}", typeId);
					continue;
				}

				JToken token = property.Value;
				if (token.Type == JTokenType.Null) {
					builder.AddRaw(type, Component.REMOVED);
					continue;
				}

				try {
					object value = type.FromJson(token);
					builder.AddRaw(type, value);
				} catch (Exception e) {
					Logger.LogFatal(e, "Unable to parse component value {} (component type: {})", token.ToString(Formatting.None), typeId);
				}
			}

			return builder.Build();
		}

		public override bool Equals(object obj) {
			return obj is ComponentChanges other && this.Equals(other);
		}

		public bool Equals(ComponentChanges other) {
			if (this.Size() != other.Size()) return false;
			return this.changedComponents.All(kvp => other.changedComponents.TryGetValue(kvp.Key, out object v) && kvp.Value.Equals(v));
		}

		public override int GetHashCode() {
			int hash = 0;
			foreach (var kvp in this.changedComponents.OrderBy(k => k.Key.GetHashCode())) {
				hash = HashCode.Combine(hash, kvp.Key, kvp.Value);
			}
			return hash;
		}

		public override string ToString() => ToString(this.changedComponents);

		private static string ToString(Dictionary<ComponentType, object> changes) {
			return "{" + string.Join(", ", changes.Select(kvp => {
				string v = Component.IsRemoved(kvp.Value) ? "REMOVED" : kvp.Value.ToString();
				return $"{kvp.Key}[{v}]";
			})) + "}";
		}

		public sealed record AddedRemovedPairs(IComponentMap added, HashSet<ComponentType> removed);

		public sealed class Builder {
			private readonly Dictionary<ComponentType, object> changedComponents = new();

			public ComponentChanges Build() => new(this.changedComponents);

			public Builder Add<T>(Component<T> component) => this.Add(component.type, component.value);

			public Builder Add<T>(ComponentType<T> type, T value) {
				this.changedComponents[type] = value!;
				return this;
			}

			public Builder Remove(ComponentType type) {
				this.changedComponents[type] = Component.REMOVED;
				return this;
			}

			internal Builder AddRaw(ComponentType type, object value) {
				this.changedComponents[type] = value;
				return this;
			}
		}
	}
}
