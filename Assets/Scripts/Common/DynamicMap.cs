using SoulboundBackend.Client.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SoulboundBackend.Common.Collections {
	public class DynamicMap<T> : Dictionary<string, T> {
		public DynamicMap() : base(StringComparer.OrdinalIgnoreCase) {
		}

		public void AddRange(DynamicMap<T> other) {
			foreach (var kvp in other) {
				this[kvp.Key] = kvp.Value;
			}
		}

		public void AddRange(object other) {
			if (other == null) {
				return;
			}

			var properties = other.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
			foreach (var property in properties) {
				if (property.GetValue(other) is T value) {
					this[property.Name] = value;
				} else {
					Logger.LogError(new InvalidOperationException($"Property '{property.Name}' is not of type {typeof(T).Name}"));
				}
			}
		}

		public IEnumerable<T> ValueList() => this.Values;
	}
}

