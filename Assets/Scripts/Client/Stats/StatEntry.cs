using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Common;
using SoulboundBackend.Common.Json;
using SoulboundBackend.Common.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;

#nullable enable

namespace SoulboundBackend.Client.Stats {
	public class StatEntry<TValue> : IStatEntry where TValue : struct, IComparable<TValue> {
		private static readonly Logger logger = Logger.CreateInstance();
		[JsonProperty]
		[JsonConverter(typeof(ModifierDictionaryConverter))]
		private Dictionary<IStatProvider, List<AbstractValueModifier>> modifiers = new();
		public event Action<StatEntry<TValue>>? OnModifiersChanged;
		public TValue baseValue { get; protected set; }
		public StatDefinition<TValue> definition { get; protected set; }
		private IStatProcessor<TValue>? _processor;
		public IStatProcessor<TValue> processor => _processor ?? definition.defaultProcessor;
		private bool flag_blockUpdate = false;

		public StatEntry(TValue baseValue, StatDefinition<TValue> definition, IStatProcessor<TValue> processor) 
			: this(baseValue, definition) {
			this._processor = processor;
		}

		public StatEntry(TValue baseValue, StatDefinition<TValue> definition) {
			this.baseValue = baseValue;
			this.definition = definition;
		}

		public TValue GetProcessedValue() {
			try {
				IEnumerable<ValueModifier<TValue>> casted = modifiers.SelectMany(m => m.Value.Cast<ValueModifier<TValue>>());
				return this.processor.ProcessFinalValue(baseValue, casted);
			} catch (InvalidCastException e) {
				logger.LogError(e);
				return default(TValue);
			}
		}

		public void Add(AbstractValueModifier serializableStat, IStatProvider provider) {
			if (Validate(serializableStat, provider)) {
				if (!modifiers.ContainsKey(provider)) {
					modifiers.Add(provider, new List<AbstractValueModifier>());
				}
				modifiers[provider].Add(serializableStat as ValueModifier<TValue>);
			}
			InvocationHelper.If(!flag_blockUpdate, () => OnModifiersChanged?.Invoke(this));
		}

		public void AddRange(params (AbstractValueModifier stat, IStatProvider provider)[] modifiers) {
			flag_blockUpdate = true;
			modifiers.ToList().ForEach(modifier => this.Add(modifier.stat, modifier.provider));
			OnModifiersChanged?.Invoke(this);
			flag_blockUpdate = false;
		}

		public void Remove(AbstractValueModifier serializableStat, IStatProvider provider) {
			if (Validate(serializableStat, provider) && modifiers.TryGetValue(provider, out var stats)) {
				stats.Remove(serializableStat as ValueModifier<TValue>);
				if (stats.Count == 0) {
					modifiers.Remove(provider);
				}
			}
			InvocationHelper.If(!flag_blockUpdate, () => OnModifiersChanged?.Invoke(this));
		}

		public void RemoveRange(params (AbstractValueModifier stat, IStatProvider provider)[] modifiers) {
			flag_blockUpdate = true;
			modifiers.ToList().ForEach(modifier => this.Remove(modifier.stat, modifier.provider));
			OnModifiersChanged?.Invoke(this);
			flag_blockUpdate = false;
		}

		public void SetModifiers(List<(AbstractValueModifier stat, IStatProvider provider)> modifiers) {
			this.modifiers.Clear();
			this.AddRange(modifiers.ToArray());
		}

		public object GetBoxedValue() => this.GetProcessedValue();

		public List<(AbstractValueModifier, IStatProvider)> GetBoxedModifiers() {
			return modifiers.Cast<(AbstractValueModifier, IStatProvider)>().ToList();
		}

		private bool Validate(AbstractValueModifier serializableStat, IStatProvider provider) {
			string failedApplication = "Failed to apply serializedStat:";
			if (provider == null) {
				logger.ThrowException(null, new ArgumentException($"{failedApplication} Null IStatProvider provider called on serializable stat {serializableStat}"));
				return false;
			}

			StatDefinition<TValue> definition = (StatDefinition<TValue>)serializableStat.statDefinition;
			if (definition != this.definition) {
				logger.ThrowException(null, new ArgumentException($"{failedApplication} Mismatched stat definition references for serializable stat {serializableStat} and stat entry {this}"));
				return false;
			}
			if (!definition.supportedApplications.Supports(serializableStat.applicationType)) {
				logger.ThrowException(null, new ArgumentException($"{failedApplication} Unsupported application type {serializableStat.applicationType} for stat definition {definition}"));
				return false;
			}

			return true;
		}

		public override string ToString() {
			string modifiers = this.modifiers.ToString();
			if (string.IsNullOrEmpty(modifiers)) {
				modifiers = "null";
			}
			return $"StatEntry[type: {typeof(TValue)}, baseValue: {baseValue}, definitionReference: {definition}, modifiers: {modifiers}]";
		}

	}

	sealed class ModifierDictionaryConverter : JsonDictionaryConverter<IStatProvider, List<AbstractValueModifier>> {
		public override void WriteJson(JsonWriter writer, Dictionary<IStatProvider, List<AbstractValueModifier>> value, JsonSerializer serializer) {
			writer.WriteStartArray();
			foreach (var kvp in value) {
				//var persistentStats = kvp.Value.Where(stat => stat.persistent).ToList();
				//if (persistentStats.Count == 0) {
				//	continue;
				//}

				writer.WriteStartObject();
				writer.WritePropertyName("key");
				serializer.Serialize(writer, kvp.Key, typeof(IStatProvider));
				writer.WritePropertyName("value");
				serializer.Serialize(writer, kvp.Value, typeof(List<AbstractValueModifier>));
				//serializer.Serialize(writer, persistentStats, typeof(List<AbstractValueModifier>));
				writer.WriteEndObject();
			}
			writer.WriteEndArray();
		}
	}
}