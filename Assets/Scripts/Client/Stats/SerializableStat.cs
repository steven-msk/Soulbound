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
	[SerializeField] private SerializedStatReference serializedReference;
	[SerializeField] private StatValueType valueType;
	[SerializeField] private StatApplicationType applicationType;
	[SerializeField] private string value;
	[SerializeField] private bool applyAsBonus;
	public SerializedStatReference SerializedReference => serializedReference;
	public StatApplicationType ApplicationType => applicationType;
	public bool ApplyAsBonus => applyAsBonus;

	public SerializableStat(SerializedStatReference serializedReference, StatValueType valueType, StatApplicationType appliance, string value, bool applyAsBonus) {
		this.serializedReference = serializedReference;
		this.valueType = valueType;
		this.applicationType = appliance;
		this.value = value;
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


