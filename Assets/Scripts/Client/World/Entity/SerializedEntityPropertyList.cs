using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

public sealed class SerializedEntityPropertyList : List<AbstractSerializedEntityProperty> {
	public SerializedEntityPropertyList() : base() {
	}

	public static SerializedEntityPropertyList From(List<AbstractSerializedEntityProperty> list) {
		var propertyList = new SerializedEntityPropertyList();
		propertyList.AddRange(list);
		return propertyList;
	}

	public static SerializedEntityPropertyList Empty() => new();

	public TValue Get<TValue>(string key, TValue defalultValue = default) {
		var property = this.FirstOrDefault(property => property.GetKey() == key);
		if (property == default) {
			return defalultValue;
		}
		return property.GetValue<TValue>();
	}

	public TValue GetOrThrow<TValue>(string key) {
		TValue value = this.Get<TValue>(key);
		if (value == null) {
			throw new ArgumentException($"Could not find property with key {key} in entity property list");
		}
		return value;
	}

	public SerializedEntityPropertyList Add<TValue>(string key, TValue value) {
		this.Add(new SerializedEntityProperty<TValue>(key, value));
		return this;
	}

	public SerializedEntityPropertyList AddRange<TValue>(params (string key, TValue value)[] range) {
		foreach (var item in range) {
			this.Add(item.key, item.value);
		}
		return this;
	}

	public bool ContainsKey(string key) => this.Any(prop => prop.GetKey() == key);

	public void Set<TValue>(string key, TValue value) {
		var property = this.FirstOrDefault(p => p.GetKey() == key);
		if (property == null) {
			this.Add(new SerializedEntityProperty<TValue>(key, value));
		} else {
			property.SetValueFromObject(value);
		}
	}
}