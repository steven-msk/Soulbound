using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

public class DynamicMap<T> : Dictionary<string, T> {
	private static readonly Logger logger = Logger.CreateInstance();
	public DynamicMap() : base(StringComparer.OrdinalIgnoreCase) {
	}

	public void AddRange(DynamicMap<T> other) {
		foreach (var kvp in other) {
			this[kvp.Key] = kvp.Value;
		}
	}

	public void AddRange(object source) {
		if (source == null) {
			return;
		}

		var properties = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
		foreach (var property in properties) {
			if (property.GetValue(source) is T value) {
				this[property.Name] = value;
			} else {
				logger.LogError(null, new InvalidOperationException($"Property '{property.Name}' is not of type {typeof(T).Name}"));
			}
		}
	}

	public IEnumerable<T> ValueList() => this.Values;
}
