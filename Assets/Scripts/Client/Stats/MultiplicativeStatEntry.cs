using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;

public class MultiplicativeStatEntry<TProcessor, TValue> : AbstractStatEntry<TValue> where TProcessor : IMultiplicativeStatProcessor<TValue>, new() {
	private readonly List<TValue> flatBonuses = new();
	private readonly List<float> percentageBonuses = new();
	private readonly TProcessor processor = new();

	public MultiplicativeStatEntry(TValue baseValue, StatType<TValue> typeReference, Action<StatType<TValue>, AbstractStatEntry<TValue>> onInstantiateAction = null) 
		: base(baseValue, typeReference, onInstantiateAction) {
	}

	public void AddFlatBonus(TValue value) => flatBonuses.Add(value);

	public void AddPercentageBonus(float value) => percentageBonuses.Add(value);

	public float GetValue() => processor.CalculateFinalValue(BaseValue, flatBonuses, percentageBonuses);

	public override void ApplyToSerialized(SerializableStat serializableStat) {
		var value = serializableStat.GetValue();
		if (serializableStat.ApplicationType == SerializableStat.StatApplicationType.Flat && value is TValue flatValue) {
			this.AddFlatBonus(flatValue);
		} else if (serializableStat.ApplicationType == SerializableStat.StatApplicationType.Percentage && value is float percentageValue) {
			this.AddPercentageBonus(percentageValue);
		} else {
			Debug.Log($"Could not apply serialized stats to {this}, unsupported value type {value.GetType()} for entry of type {typeof(TValue)}");
		}
	}
}
