public abstract class AbstractStatEntry<TValue> {
	public TValue BaseValue { get; protected set; }
	public StatType<TValue> TypeReference { get; protected set; }

	protected AbstractStatEntry(TValue baseValue, StatType<TValue> typeReference) {
		this.BaseValue = baseValue;
		this.TypeReference = typeReference;
	}
}