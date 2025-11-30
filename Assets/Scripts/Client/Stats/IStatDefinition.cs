using System;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;

#nullable enable

namespace SoulboundBackend.Client.Stats {

	//[JsonConverter(typeof(IStatDefinition.StatDefinitionJsonConverter))]
	public interface IStatDefinition {
		string baseName { get; }
		Type valueType { get; }
		SupportedApplicationType supportedApplications { get; }
		string? id { get; set; }

		//string GetFormattedName(object value);

		//string GetFormattedValue(object value, bool applyAsBonus);

		//static Dictionary<string, IStatDefinition> definitionsById = new();

		//static IStatDefinition ByID(string id) {
		//	if (definitionsById.TryGetValue(id, out IStatDefinition definition)) {
		//		return definition;
		//	}
		//	throw new ArgumentException($"No stat definition found for id {id}");
		//}

		//internal static void Register(string id, IStatDefinition definition) {
		//	definitionsById[id] = definition;
		//}

		//virtual string GetFormattedExpression(object value, bool applyAsBonus = false) {
		//	return $"{this.GetFormattedValue(value, applyAsBonus)} {this.GetFormattedName(value)}";
		//}

		public bool SupportsApplication(StatApplicationType applicationType) {
			return supportedApplications.HasFlag(applicationType);
		}

		//public sealed class StatDefinitionJsonConverter : JsonConverter<IStatDefinition> {
		//	public override IStatDefinition? ReadJson(JsonReader reader, Type objectType, IStatDefinition? existingValue, bool hasExistingValue, JsonSerializer serializer) {
		//		string? id = (string?)reader.Value;
		//		return ByID(id!);
		//	}

		//	public override void WriteJson(JsonWriter writer, IStatDefinition? value, JsonSerializer serializer) {
		//		writer.WriteValue(value!.id);
		//	}
		//}
	}
}
