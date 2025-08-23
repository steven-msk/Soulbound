using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;

public class MultiplicativeStatEntry<TValue> : AbstractStatEntry<TValue> where TValue : struct, IComparable<TValue> {
	private readonly List<(TValue value, IStatProvider source)> flatBonuses = new();
	private readonly List<(float value, IStatProvider source)> percentageBonuses = new();
	private readonly IMultiplicativeStatProcessor<TValue> processor;

	public MultiplicativeStatEntry(TValue baseValue, StatDefinition<TValue> typeReference, IMultiplicativeStatProcessor<TValue> processor,
			Action<StatDefinition<TValue>, AbstractStatEntry<TValue>> onInstantiateAction = null) 
		: base(baseValue, typeReference, onInstantiateAction) {
		this.processor = processor;
	}

	public void AddFlatBonus(TValue value, IStatProvider source) => flatBonuses.Add((value, source));

	public void AddPercentageBonus(float value, IStatProvider source) => percentageBonuses.Add((value, source));

	public void RemoveFlatBonus(TValue value, IStatProvider source) => flatBonuses.Remove((value, source));

	public void RemovePercentageBonus(float value, IStatProvider source) => percentageBonuses.Remove((value, source));

	public float GetProcessedValue() {
		return processor.ProcessFinalValue(BaseValue, flatBonuses.Select(bonus => bonus.value), percentageBonuses.Select(bonus => bonus.value));
	}

	public override object Abstract_GetProcessedValue() => GetProcessedValue();

	public override void ApplyToSerialized(AbstractSerializableStat serializableStat, IStatProvider source) { 
		Invoke(serializableStat, source, AddFlatBonus, AddPercentageBonus);
	}

	public override void RevokeToSerialized(AbstractSerializableStat serializableStat, IStatProvider source) { 
		Invoke(serializableStat, source, RemoveFlatBonus, RemovePercentageBonus);
	}

	private void Invoke(AbstractSerializableStat serializableStat, IStatProvider source, Action<TValue, IStatProvider> flatAction, Action<float, IStatProvider> percentageAction) {
		var value = serializableStat.GetBoxedValue();
		if (serializableStat.GetApplicationType().Supports(StatApplicationType.Flat) && value is TValue flatValue) {
			flatAction.Invoke(flatValue, source);
		} else if (serializableStat.GetApplicationType().Supports(StatApplicationType.Percentage) && value is float percentageValue) {
			percentageAction.Invoke(percentageValue, source);
		} else {
			throw new IStatEntry.UnsupportedSerializableStatTypeException(value, typeof(TValue));
		}
	}
}
