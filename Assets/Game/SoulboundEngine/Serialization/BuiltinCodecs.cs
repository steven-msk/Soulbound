namespace SoulboundEngine.Serialization {
	using Newtonsoft.Json.Linq;

	public static class BuiltinCodecs {
		public static readonly Codec<int> INT = new SimpleCodec<int>(
			stringEncoder: v => v.ToString(),
			decoder: json => json.Type is JTokenType.Integer
				? DataResult<int>.Success((int)json)
				: DataResult<int>.Error($"Expected int, got {json.Type}")
		);

		public static readonly Codec<double> DOUBLE = new SimpleCodec<double>(
			stringEncoder: v => v.ToString(),
			decoder: json => json.Type is JTokenType.Float or JTokenType.Integer
				? DataResult<double>.Success((double)json)
				: DataResult<double>.Error($"Expected double, got {json.Type}")
		);

		public static readonly Codec<float> FLOAT = new SimpleCodec<float>(
			stringEncoder: v => v.ToString(),
			decoder: json => json.Type is JTokenType.Float or JTokenType.Integer
				? DataResult<float>.Success((float)json)
				: DataResult<float>.Error($"Expected float, got {json.Type}")
		);

		public static readonly Codec<string> STRING = new SimpleCodec<string>(
			stringEncoder: v => v,
			decoder: json => json.Type is JTokenType.String
				? DataResult<string>.Success((string)json)
				: DataResult<string>.Error($"Expected string, got {json.Type}")
		);
	}
}
