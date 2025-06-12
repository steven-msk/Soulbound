using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SimpleStatEntry<TProcessor, TValue> : AbstractStatEntry<TValue> where TProcessor : IStatProcessor<TValue>, new() {
	private readonly List<TValue> bonuses = new();
	private readonly TProcessor processor = new();

	public SimpleStatEntry(TValue baseValue, StatType<TValue> typeReference, Action<StatType<TValue>, AbstractStatEntry<TValue>> onInstantiateAction = null) 
		: base(baseValue, typeReference, onInstantiateAction) {
	}

	public void AddBonus(TValue value) => bonuses.Add(value);

	public override void ApplyToSerialized(SerializableStat serializableStat) => this.AddBonus(serializableStat.GetValue<TValue>());

	public TValue GetValue() => processor.CalculateFinalValue(BaseValue, bonuses);
}
