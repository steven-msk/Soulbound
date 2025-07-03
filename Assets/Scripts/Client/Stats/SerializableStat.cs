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
	[SerializeField] protected SerializedStatReference serializedReference;
	public SerializedStatReference SerializedReference => serializedReference;

	[SerializeField] protected StatValueType valueType;
	public StatValueType ValueType => valueType;

	[SerializeField] protected StatApplicationType applicationType;
	public StatApplicationType ApplicationType => applicationType;

	[SerializeField] protected string value;
	public string RawValue => value;

	[SerializeField] protected bool applyAsBonus;
	public bool ApplyAsBonus => applyAsBonus;

	public SerializableStat(SerializedStatReference serializedReference, StatValueType valueType, StatApplicationType appliance, object value, bool applyAsBonus) {
		this.serializedReference = serializedReference;
		this.valueType = valueType;
		this.applicationType = appliance;
		this.value = value.ToString();
		this.applyAsBonus = applyAsBonus;
	}

	[CanBeNull] public object GetValue() {
		return valueType switch {
			StatValueType.Int => TryGetValue(value => Convert.ToInt32(value)),
			StatValueType.Float => TryGetValue(value => Convert.ToSingle(value)),
			_ => null
		};
	}

	private object TryGetValue(Func<string, object> action) {
		try {
			return action.Invoke(value);
		} catch (FormatException) {
			Debug.LogError($"Invalid stat value {value} for type {valueType.ToInternalType()}. Did you type the wrong format?");
			return null;
		}
	}

	[CanBeNull] public TValue GetValue<TValue>() => (TValue)GetValue();

	public string GetFormattedExpression() => serializedReference.ToStatType().GetFormattedExpression(this.GetValue(), applyAsBonus);

	[Serializable]
	public enum StatValueType {
		Int,
		Float
	}
	
	[Serializable]
	public enum StatApplicationType {
		Flat,
		Percentage
	}
}

public static class ValueTypeIdentification {
	public static Type ToInternalType(this SerializableStat.StatValueType valueType) {
		return valueType switch {
			SerializableStat.StatValueType.Int => typeof(int),
			SerializableStat.StatValueType.Float => typeof(float),
			_ => null
		};
	}
}


