using System;

public abstract class AbstractStatEntry<TValue> : IStatEntry {
	public TValue BaseValue { get; protected set; }
	public StatDefinition<TValue> TypeReference { get; protected set; }

	protected AbstractStatEntry(TValue baseValue, StatDefinition<TValue> typeReference, Action<StatDefinition<TValue>, AbstractStatEntry<TValue>> onInstantiateAction = null) {
		this.BaseValue = baseValue;
		this.TypeReference = typeReference;
		onInstantiateAction?.Invoke(typeReference, this);
	}

	public abstract object Abstract_GetProcessedValue();

	public abstract void ApplyToSerialized(AbstractSerializableStat serializableStat, IStatProvider source);

	public abstract void RevokeToSerialized(AbstractSerializableStat serializableStat, IStatProvider source);
}