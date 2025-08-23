using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class StatEntry<TValue> : IStatEntryImpl where TValue : struct, IComparable<TValue> {
	private static readonly Logger logger = Logger.CreateInstance();
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

	public void Add(AbstractSerializableStat serializableStat, IStatProvider source) {
		if (Validate(serializableStat, source)) {
			modifiers.Add(((SerializableStat<TValue>)serializableStat, source));
		}
	}

	public void Remove(AbstractSerializableStat serializableStat, IStatProvider source) {
		if (Validate(serializableStat, source)) {
			modifiers.Remove(((SerializableStat<TValue>)serializableStat, source));
		}
	}

	public object GetBoxedValue() => this.GetProcessedValue();

	private bool Validate(AbstractSerializableStat serializableStat, IStatProvider source) {
		string failedApplication = "Failed to apply serializedStat:";
		if (source == null) {
			logger.ThrowException(null, new ArgumentException($"{failedApplication} Null IStatProvider source called on serializable stat {serializableStat}"));
			return false;
		}

		StatDefinition<TValue> definition = (StatDefinition<TValue>)serializableStat.GetStatDefinition();
		if (definition != this.definitionReference) {
			logger.ThrowException(null, new ArgumentException($"{failedApplication} Mismatched stat definition references for serializable stat {serializableStat} and stat entry {this}"));
			return false;
		}
		if (!definition.supportedApplications.Supports(serializableStat.GetApplicationType())) {
			logger.ThrowException(null, new ArgumentException($"{failedApplication} Unsupported application type {serializableStat.GetApplicationType()} for stat definition {definition}"));
			return false;
		}

		return true;
	}

	public override string ToString() {
		string modifiers = string.Join(", ", this.modifiers.Select(m => $"{m.source}:{m.stat}"));
		if (string.IsNullOrEmpty(modifiers)) {
			modifiers = "null";
		}
		return $"StatEntry[type: {typeof(TValue)}, baseValue: {baseValue}, definitionReference: {definitionReference}, modifiers: {modifiers}]";
	}
}