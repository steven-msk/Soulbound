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
	[SerializeField] protected StatValueType valueType;
	[SerializeField] protected StatApplicationType applicationType;
	[SerializeField] protected string value;
	[SerializeField] protected bool applyAsBonus;
	public SerializedStatReference SerializedReference => serializedReference;
	public StatApplicationType ApplicationType => applicationType;
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
			StatValueType.Int => Convert.ToInt32(value),
			StatValueType.Float => (float)Convert.ToDouble(value),
			_ => null
		};
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


