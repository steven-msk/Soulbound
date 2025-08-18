using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SimpleStatEntry<TProcessor, TValue> : AbstractStatEntry<TValue> where TProcessor : IStatProcessor<TValue>, new() {
	private readonly List<(TValue value, IStatProvider source)> bonuses = new();
	private readonly TProcessor processor = new();

	public SimpleStatEntry(TValue baseValue, StatType<TValue> typeReference, Action<StatType<TValue>, AbstractStatEntry<TValue>> onInstantiateAction = null) 
		: base(baseValue, typeReference, onInstantiateAction) {
	}

	public void AddBonus(TValue value, IStatProvider source) => bonuses.Add((value, source));

	public void RemoveBonus(TValue value, IStatProvider source) => bonuses.Remove((value, source));

	public override void ApplyToSerialized(SerializableStat serializableStat, IStatProvider source) => this.AddBonus(serializableStat.GetValue<TValue>(), source);

	public override void RevokeToSerialized(SerializableStat serializableStat, IStatProvider source) => this.RemoveBonus(serializableStat.GetValue<TValue>(), source);

	public TValue GetProcessedValue() => processor.ProcessFinalValue(BaseValue, bonuses.Select(bonus => bonus.value));

	public override object Abstract_GetProcessedValue() => GetProcessedValue();
}
