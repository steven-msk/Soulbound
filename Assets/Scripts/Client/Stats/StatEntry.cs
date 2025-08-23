using System;
using System.Collections.Generic;
using System.Linq;

public class StatEntry<TValue> : List<(SerializableStat<TValue> stat, IStatProvider source)>, IStatEntryImpl
		where TValue : struct, IComparable<TValue> {
	public TValue baseValue { get; protected set; }
	public StatDefinition<TValue> definitionReference { get; protected set; }

	public StatEntry(TValue baseValue, StatDefinition<TValue> definitionReference) {
		this.baseValue = baseValue;
		this.definitionReference = definitionReference;
	}

	public TValue GetProcessedValue() {
		return this.definitionReference.processor.ProcessFinalValue(baseValue, this.Select(m => m.stat));
	}

	public void AddModifier(AbstractSerializableStat serializableStat, IStatProvider source) {
		this.Add(((SerializableStat<TValue>)serializableStat, source));
	}

	public void RemoveModifier(AbstractSerializableStat serializableStat, IStatProvider source) {
		this.Remove(((SerializableStat<TValue>)serializableStat, source));
	}

	public object GetBoxedValue() => this.GetProcessedValue();
}