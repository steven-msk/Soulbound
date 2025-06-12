using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer.Internal;
using UnityEngine;
using UnityEngine.SearchService;

[Serializable]
public class SerializableStat {
	public SerializedStatReference serializedReference;
	public StatValueType valueType;
	public StatValueAppliance appliance;
	public string value;

	[CanBeNull] public object GetValue() {
		return valueType switch {
			StatValueType.Int => Convert.ToInt32(value),
			StatValueType.Float => (float)Convert.ToDouble(value),
			_ => null
		};
	}

	[CanBeNull] public TValue GetValue<TValue>() => (TValue)GetValue();

	[Serializable]
	public enum StatValueType {
		Int,
		Float
	}
	
	[Serializable]
	public enum StatValueAppliance {
		Flat,
		Percentage
	}
}


