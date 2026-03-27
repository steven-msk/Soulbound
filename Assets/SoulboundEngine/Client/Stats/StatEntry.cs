using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Common.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

#nullable enable

namespace SoulboundEngine.Client.Stats {
	public class StatEntry<TValue> : IStatEntry where TValue : struct, IComparable<TValue> {
		private Dictionary<ModificationToken, List<IStatEntryModifier<TValue>>> modifiers = new();
		private Dictionary<IStatEntryModifier<TValue>, IModificationProcedure<TValue>> modificationProcedues = new();
		public TValue baseValue { get; private set; }
		public StatDefinition<TValue> definition { get; private set; }
		IStatDefinition IStatEntry.definition => definition;

		private readonly IStatProcessor<TValue> processor;
		Type IStatEntry.valueType => typeof(TValue);

		public StatEntry(StatDefinition<TValue> definition, TValue baseValue, IStatProcessor<TValue> processor) {
			this.baseValue = baseValue;
			this.definition = definition;
			this.processor = processor;
		}

		public StatEntry(StatDefinition<TValue> definition, TValue baseValue) 
			: this(definition, baseValue, new StatProcessor<TValue>()) {
		}

		public TValue GetProcessedValue() {
			IEnumerable<IStatEntryModifier<TValue>> modifiers = this.modifiers.SelectMany(m => m.Value);
			return this.processor.ProcessFinalValue(baseValue, this, modifiers, modificationProcedues);
		}

		object IStatEntry.CalculateBoxedValue() => GetProcessedValue();

		public void AcceptModifier(IStatEntryModifier modifier, ModificationToken modificationToken) {
			modifier.Apply(this, modificationToken);
		}

		public void RemoveModifier(IStatEntryModifier modifier, ModificationToken modificationToken) {
			modifier.Remove(this, modificationToken);
		}

		public void RemoveModifiers(ModificationToken modificationToken) {
			if (this.modifiers.TryGetValue(modificationToken, out var modifiers)) {
				var clone = new List<IStatEntryModifier<TValue>>(modifiers);

				foreach (var modifier in clone) {
					modifier.Remove(this, modificationToken);
				}
			}
		}

		public void CommitModifier(IStatEntryModifier<TValue> modifier, ModificationToken modificationToken, IModificationProcedure<TValue> procedure) {
			if (!modifiers.TryGetValue(modificationToken, out var list)) {
				modifiers[modificationToken] = list = new();
			}
			list.Add(modifier);
			modificationProcedues.Add(modifier, procedure);
		}

		public void UncommitModifier(IStatEntryModifier<TValue> modifier, ModificationToken modificationToken) {
			if (modifiers.TryGetValue(modificationToken, out var list)) {
				list.Remove(modifier);
			}
			if (!list.Any()) {
				modifiers.Remove(modificationToken);
			}
			modificationProcedues.Remove(modifier);
		}

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
