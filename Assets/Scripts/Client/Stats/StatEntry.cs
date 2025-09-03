using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class StatEntry<TValue> : IStatEntryImpl where TValue : struct, IComparable<TValue> {
	private static readonly Logger logger = Logger.CreateInstance();
	[JsonProperty]
	[JsonConverter(typeof(JsonDictionaryConverter<IStatProvider, List<AbstractSerializableStat>>))]
	private readonly Dictionary<IStatProvider, List<AbstractSerializableStat>> modifiers = new();
	public TValue baseValue { get; protected set; }
	public StatDefinition<TValue> definitionReference { get; protected set; }

	public StatEntry(TValue baseValue, StatDefinition<TValue> definitionReference) {
		this.baseValue = baseValue;
		this.definitionReference = definitionReference;
	}

	public TValue GetProcessedValue() {
		try {
			IEnumerable<SerializableStat<TValue>> casted = modifiers.SelectMany(m => m.Value.Cast<SerializableStat<TValue>>());
			return this.definitionReference.processor.ProcessFinalValue(baseValue, casted);
		} catch (InvalidCastException e) {
			logger.ThrowException(null, e);
			return default(TValue);
		}
	}

	public void Add(AbstractSerializableStat serializableStat, IStatProvider provider) {
		if (Validate(serializableStat, provider)) {
			if (!modifiers.ContainsKey(provider)) {
				modifiers.Add(provider, new List<AbstractSerializableStat>());
			}
			modifiers[provider].Add(serializableStat as SerializableStat<TValue>);
		}
	}

	public void AddRange(params (AbstractSerializableStat stat, IStatProvider provider)[] modifiers) {
		modifiers.ToList().ForEach(modifier => this.Add(modifier.stat, modifier.provider));
	}

	public void Remove(AbstractSerializableStat serializableStat, IStatProvider provider) {
		if (Validate(serializableStat, provider) && modifiers.TryGetValue(provider, out var stats)) {
			stats.Remove(serializableStat as SerializableStat<TValue>);
			if (stats.Count == 0) {
				modifiers.Remove(provider);
			}
		}
	}

	public void RemoveRange(params (AbstractSerializableStat stat, IStatProvider provider)[] modifiers) {
		modifiers.ToList().ForEach(modifier => this.Remove(modifier.stat, modifier.provider));
	}

	public void SetModifiers(List<(AbstractSerializableStat stat, IStatProvider provider)> modifiers) {
		this.modifiers.Clear();
		this.AddRange(modifiers.ToArray());
	}

	public object GetBoxedValue() => this.GetProcessedValue();

	public List<(AbstractSerializableStat, IStatProvider)> GetBoxedModifiers() {
		return modifiers.Cast<(AbstractSerializableStat, IStatProvider)>().ToList();
	}

	private bool Validate(AbstractSerializableStat serializableStat, IStatProvider provider) {
		string failedApplication = "Failed to apply serializedStat:";
		if (provider == null) {
			logger.ThrowException(null, new ArgumentException($"{failedApplication} Null IStatProvider provider called on serializable stat {serializableStat}"));
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
		string modifiers = string.Join(", ", this.modifiers.SelectMany(m => $"{m.Key}:{m.Value}"));
		if (string.IsNullOrEmpty(modifiers)) {
			modifiers = "null";
		}
		return $"StatEntry[type: {typeof(TValue)}, baseValue: {baseValue}, definitionReference: {definitionReference}, modifiers: {modifiers}]";
	}
}