using ModestTree;
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
		private Dictionary<ModificationToken, List<IStatEntryModifier<TValue>>> modifiers = new();
		[Obsolete]
		public event Action<StatEntry<TValue>>? OnModifiersChanged;
		public TValue baseValue { get; protected set; }
		public StatDefinition<TValue> definition { get; protected set; }
		private IStatProcessor<TValue>? _processor;
		public IStatProcessor<TValue> processor => _processor ?? definition.defaultProcessor;
		Type IStatEntry.valueType => typeof(TValue);
		[Obsolete]
		private bool flag_blockUpdate = false;

		public StatEntry(StatDefinition<TValue> definition, TValue baseValue, IStatProcessor<TValue> processor) 
			: this(definition, baseValue) {
			this._processor = processor;
		}

		public StatEntry(StatDefinition<TValue> definition, TValue baseValue) {
			this.baseValue = baseValue;
			this.definition = definition;
		}

		[Obsolete]
		public TValue GetProcessedValue() {
			try {
				IEnumerable<ValueModifier<TValue>> casted = modifiers.SelectMany(m => m.Value.Cast<ValueModifier<TValue>>());
				return this.processor.ProcessFinalValue(baseValue, casted);
			} catch (InvalidCastException e) {
				logger.LogError(e);
				return default(TValue);
			}
		}

		public void AcceptModifier(IStatEntryModifier modifier, ModificationToken modificationToken) {
			modifier.Apply(this, modificationToken);
		}

		public void RemoveModifier(IStatEntryModifier modifier, ModificationToken modificationToken) {
			modifier.Remove(this, modificationToken);
		}

		public void RemoveModifiers(ModificationToken modificationToken) {
			foreach (var modifier in modifiers[modificationToken]) {
				modifier.Remove(this, modificationToken);
			}
		}

		public void CommitModifier(IStatEntryModifier<TValue> modifier, ModificationToken modificationToken) {
			if (!modifiers.TryGetValue(modificationToken, out var list)) {
				modifiers[modificationToken] = list = new();
			}
			list.Add(modifier);
		}

		public void UncommitModifier(IStatEntryModifier<TValue> modifier, ModificationToken modificationToken) {
			if (modifiers.TryGetValue(modificationToken, out var list)) {
				list.Remove(modifier);
			}
			if (list.IsEmpty()) {
				modifiers.Remove(modificationToken);
			}
		}

		public object CalculateBoxedValue() => this.GetProcessedValue();

		public override string ToString() {
			string modifiers = this.modifiers.ToString();
			if (string.IsNullOrEmpty(modifiers)) {
				modifiers = "null";
			}
			return $"StatEntry[type: {typeof(TValue)}, baseValue: {baseValue}, definitionReference: {definition}, modifiers: {modifiers}]";
		}

	}

	[Obsolete]
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