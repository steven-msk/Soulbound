using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using Unity.VisualScripting;

#nullable enable

[JsonConverter(typeof(IStatDefinitionImpl.StatDefinitionJsonConverter))]
public interface IStatDefinitionImpl {
	string baseName { get; }
	Type valueType { get; }
	SupportedApplicationType supportedApplications { get; }
	string? id { get; set; }

	string GetFormattedName(object value);

	string GetFormattedValue(object value, bool applyAsBonus);

	static Dictionary<string, IStatDefinitionImpl> registered = new();

	static IStatDefinitionImpl ByID(string id) {
		if (registered.TryGetValue(id, out IStatDefinitionImpl definition)) {
			return definition;
		}
		throw new ArgumentException($"No stat definition found for id {id}");
	}

	internal static void Register(string id, IStatDefinitionImpl definition) {
		registered[id] = definition;
	}
		 
	virtual string GetFormattedExpression(object value, bool applyAsBonus = false) {
		return $"{this.GetFormattedValue(value, applyAsBonus)} {this.GetFormattedName(value)}";
	}

	public bool SupportsApplication(StatApplicationType applicationType) {
		return supportedApplications.HasFlag(applicationType);
	}

	public sealed class StatDefinitionJsonConverter : JsonConverter<IStatDefinitionImpl> {
		public override IStatDefinitionImpl? ReadJson(JsonReader reader, Type objectType, IStatDefinitionImpl? existingValue, bool hasExistingValue, JsonSerializer serializer) {
			string? id = (string?)reader.Value;
			return ByID(id!);
		}

		public override void WriteJson(JsonWriter writer, IStatDefinitionImpl? value, JsonSerializer serializer) {
			writer.WriteValue(value!.id);
		}
	}
}