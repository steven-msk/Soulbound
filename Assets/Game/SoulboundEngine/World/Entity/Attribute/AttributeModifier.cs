namespace SoulboundEngine.World.Entity.Attribute {
	using Newtonsoft.Json.Linq;
	using SoulboundEngine.Registry;
	using SoulboundEngine.Serialization;
	using System;
	using System.Collections.Generic;

#nullable enable

	public record AttributeModifier(Identifier id, double amount, AttributeModifier.Operation operation) {
		public static readonly Codec<AttributeModifier> CODEC = RecordCodec<AttributeModifier, Identifier, double, Operation>.Of(
			Field.Required<AttributeModifier, Identifier>("id", Identifier.CODEC, m => m.id),
			Field.Required<AttributeModifier, double>("amount", BuiltinCodecs.DOUBLE, m => m.amount),
			Field.Required<AttributeModifier, Operation>("operation", Operation.CODEC, m => m.operation),
			(id, amount, operation) => new AttributeModifier(id, amount, operation)
		);

		public bool Matches(Identifier id) => id.Equals(this.id);

		public JToken ToJson() {
			return new JObject() {
				["id"] = this.id.ToString(),
				["amount"] = this.amount,
				["operation"] = this.operation.serializedName
			};
		}

		public static JArray ListToJson(List<AttributeModifier> modifiers) {
			JArray array = new();
			foreach (AttributeModifier modifier in modifiers) {
				array.Add(modifier.ToJson());
			}
			return array;
		}

		public static AttributeModifier FromJson(JToken json) {
			if (json is not JObject obj) throw new ArgumentException("Attribute json is not object: " + json);

			string idString = (string?)json["id"] ?? throw new NotSupportedException("No id on attribute modifier json: " + json);
			Identifier id = Identifier.Of(idString);
			double amount = (double?)json["amount"] ?? throw new NotSupportedException("No amount on attribute modifier json: " + json);
			string operationString = (string?)json["operation"] ?? throw new NotSupportedException("No operation on attribute modifier json: " + json);
			Operation operation = Operation.BySerializedName(operationString) ?? throw new NotSupportedException("Unknown operation: " + operationString);

			return new AttributeModifier(id, amount, operation);
		}

		public static IEnumerable<AttributeModifier> ListFromJson(JToken json) {
			if (json is not JArray array) throw new ArgumentException("Attributes list json is not array: " + json);

			foreach (JToken token in array) {
				yield return FromJson(token);
			}
		}

		public readonly struct Operation {
			public static readonly Codec<Operation> CODEC = BuiltinCodecs.STRING.FlatXmap(
				encode: o => o.serializedName,
				decode: s => BySerializedName(s) is { } operation
					? DataResult<Operation>.Success(operation)
					: DataResult<Operation>.Error($"Invalid operation: {s}")
			);
			private static readonly Dictionary<string, Operation> BY_SERIALIZED_NAME = new();
			public static readonly Operation ADDITIVE = new("additive", 0);                 // +A  or -A
			public static readonly Operation ADDITIVE_PERCENT = new("additive_percent", 1); // +B% or -B%
			public static readonly Operation MULTIPLICATIVE = new("multiplicative", 2);     // xC  or x1/C
			public readonly string serializedName;
			public readonly int id;

			private Operation(string name, int id) {
				this.serializedName = name;
				this.id = id;
				BY_SERIALIZED_NAME.Add(name, this);
			}

			public static Operation? BySerializedName(string name) {
				return BY_SERIALIZED_NAME.TryGetValue(name, out Operation operation) ? operation : null;
			}
		}
	}
}
