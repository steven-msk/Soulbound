namespace SoulboundEngine.Serialization {
	using Newtonsoft.Json.Linq;
	using System;

	public static class BuiltinCodecs {
		public static readonly Codec<int> INT = new SimpleCodec<int>(
			encoder: v => v,
			decoder: json => json.Type is JTokenType.Integer
				? DataResult<int>.Success((int)json)
				: DataResult<int>.Error($"Expected int, got {json.Type}")
		);

		public static readonly Codec<double> DOUBLE = new SimpleCodec<double>(
			encoder: v => v,
			decoder: json => json.Type is JTokenType.Float or JTokenType.Integer
				? DataResult<double>.Success((double)json)
				: DataResult<double>.Error($"Expected double, got {json.Type}")
		);

		public static readonly Codec<float> FLOAT = new SimpleCodec<float>(
			encoder: v => v,
			decoder: json => json.Type is JTokenType.Float or JTokenType.Integer
				? DataResult<float>.Success((float)json)
				: DataResult<float>.Error($"Expected float, got {json.Type}")
		);

		public static readonly Codec<string> STRING = new SimpleCodec<string>(
			encoder: v => v,
			decoder: json => json.Type is JTokenType.String
				? DataResult<string>.Success((string)json)
				: DataResult<string>.Error($"Expected string, got {json.Type}")
		);

		public static readonly Codec<Guid> GUID = STRING.FlatXmap(
			encode: guid => guid.ToString(),
			decode: json => Guid.TryParse(json, out Guid guid)	
				? DataResult<Guid>.Success(guid)
				: DataResult<Guid>.Error($"Invalid guid: {guid}")
		);

		public static readonly Codec<bool> BOOLEAN = new SimpleCodec<bool>(
			encoder: v => v,
			decoder: json => json.Type is JTokenType.Boolean
				? DataResult<bool>.Success((bool)json)
				: DataResult<bool>.Error($"Expected boolean, got {json.Type}")
		);
	}
}
