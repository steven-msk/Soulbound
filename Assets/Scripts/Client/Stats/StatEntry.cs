using System;
using System.Collections.Generic;
using System.Linq;

public class StatEntry<TValue> : IStatEntryImpl where TValue : struct, IComparable<TValue> {
	private readonly List<(SerializableStat<TValue> stat, IStatProvider source)> modifiers = new();
	public TValue baseValue { get; protected set; }
	public StatDefinition<TValue> definitionReference { get; protected set; }

	public StatEntry(TValue baseValue, StatDefinition<TValue> definitionReference) {
		this.baseValue = baseValue;
		this.definitionReference = definitionReference;
	}

	public TValue GetProcessedValue() {
		return this.definitionReference.processor.ProcessFinalValue(baseValue, modifiers.Select(m => m.stat));
	}

	public void AddModifier(AbstractSerializableStat serializableStat, IStatProvider source) {
		this.modifiers.Add(((SerializableStat<TValue>)serializableStat, source));
	}

	public void RemoveModifier(AbstractSerializableStat serializableStat, IStatProvider source) {
		this.modifiers.Remove(((SerializableStat<TValue>)serializableStat, source));
	}

	public object GetBoxedValue() => this.GetProcessedValue();
}