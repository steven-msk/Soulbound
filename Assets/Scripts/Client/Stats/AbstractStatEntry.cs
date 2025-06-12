using System;

public abstract class AbstractStatEntry<TValue> : IStatEntry {
	public TValue BaseValue { get; protected set; }
	public StatType<TValue> TypeReference { get; protected set; }

	protected AbstractStatEntry(TValue baseValue, StatType<TValue> typeReference, Action<StatType<TValue>, AbstractStatEntry<TValue>> onInstantiateAction = null) {
		this.BaseValue = baseValue;
		this.TypeReference = typeReference;
		onInstantiateAction?.Invoke(typeReference, this);
	}

	public abstract void ApplyToSerialized(SerializableStat serializableStat);
}