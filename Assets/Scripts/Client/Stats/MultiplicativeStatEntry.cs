using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;

public class MultiplicativeStatEntry<TProcessor, TValue> : AbstractStatEntry<TValue> where TProcessor : IMultiplicativeStatProcessor<TValue>, new() {
	private readonly List<(TValue value, IStatProvider source)> flatBonuses = new();
	private readonly List<(float value, IStatProvider source)> percentageBonuses = new();
	private readonly TProcessor processor = new();

	public MultiplicativeStatEntry(TValue baseValue, StatType<TValue> typeReference, Action<StatType<TValue>, AbstractStatEntry<TValue>> onInstantiateAction = null) 
		: base(baseValue, typeReference, onInstantiateAction) {
	}

	public void AddFlatBonus(TValue value, IStatProvider source) => flatBonuses.Add((value, source));

	public void AddPercentageBonus(float value, IStatProvider source) => percentageBonuses.Add((value, source));

	public void RemoveFlatBonus(TValue value, IStatProvider source) => flatBonuses.Remove((value, source));

	public void RemovePercentageBonus(float value, IStatProvider source) => percentageBonuses.Remove((value, source));

	public float GetProcessedValue() => processor.ProcessFinalValue(BaseValue, flatBonuses.Select(bonus => bonus.value), percentageBonuses.Select(bonus => bonus.value));

	public override void ApplyToSerialized(SerializableStat serializableStat, IStatProvider source) => Invoke(serializableStat, source, AddFlatBonus, AddPercentageBonus);

	public override void RevokeToSerialized(SerializableStat serializableStat, IStatProvider source) => Invoke(serializableStat, source, RemoveFlatBonus, RemovePercentageBonus);

	private void Invoke(SerializableStat serializableStat, IStatProvider source, Action<TValue, IStatProvider> flatAction, Action<float, IStatProvider> percentageAction) {
		var value = serializableStat.GetValue();
		if (serializableStat.ApplicationType == SerializableStat.StatApplicationType.Flat && value is TValue flatValue) {
			flatAction.Invoke(flatValue, source);
		} else if (serializableStat.ApplicationType == SerializableStat.StatApplicationType.Percentage && value is float percentageValue) {
			percentageAction.Invoke(percentageValue, source);
		} else {
			throw new IStatEntry.UnsupportedSerializableStatTypeException(value, typeof(TValue));
		}
	}
}
