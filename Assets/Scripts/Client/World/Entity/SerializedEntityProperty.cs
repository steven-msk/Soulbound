using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;

public class SerializedEntityProperty<T> : AbstractSerializedEntityProperty {
	public string key;
	public T value;

	public SerializedEntityProperty(string key, T value) {
		this.key = key;
		this.value = value;
	}

	public override string GetKey() => key;

	public override object GetValueAsObject() => value;

	public override void SetValueFromObject(object value) {
		if (value is T typedValue) {
			this.value = typedValue;
		} else {
			throw new InvalidCastException($"Cannot assign value of type {value?.GetType()} to serialized entity property of type {typeof(T)}");
		}
	}

	public override SpawnDataValue ToSpawnDataValue() => new SpawnDataValue<T>(value);
}
