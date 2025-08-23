using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SimpleStatEntry<TValue> : AbstractStatEntry<TValue> where TValue : struct, IComparable<TValue> {
	private readonly List<(TValue value, IStatProvider source)> bonuses = new();
	private readonly IStatProcessor<TValue> processor;

	public SimpleStatEntry(TValue baseValue, StatDefinition<TValue> typeReference, IStatProcessor<TValue> processor,
			Action<StatDefinition<TValue>, AbstractStatEntry<TValue>> onInstantiateAction = null) 
		: base(baseValue, typeReference, onInstantiateAction) {
	}

	public void AddBonus(TValue value, IStatProvider source) => bonuses.Add((value, source));

	public void RemoveBonus(TValue value, IStatProvider source) => bonuses.Remove((value, source));

	public override void ApplyToSerialized(AbstractSerializableStat serializableStat, IStatProvider source) { 
		this.AddBonus((TValue)serializableStat.GetBoxedValue(), source);
	}

	public override void RevokeToSerialized(AbstractSerializableStat serializableStat, IStatProvider source) { 
		this.RemoveBonus((TValue)serializableStat.GetBoxedValue(), source); 
	}

	public TValue GetProcessedValue() => processor.ProcessFinalValue(BaseValue, bonuses.Select(bonus => bonus.value));

	public override object Abstract_GetProcessedValue() => GetProcessedValue();
}
