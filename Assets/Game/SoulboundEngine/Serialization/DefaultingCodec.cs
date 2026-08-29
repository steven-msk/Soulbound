namespace SoulboundEngine.Serialization {
	using Newtonsoft.Json.Linq;

	public record DefaultingCodec<T>(Codec<T> codec, T fallback) : Codec<T> {
		public override JToken Encode(T value) => this.codec.Encode(value);

		public override DataResult<T> Decode(JToken json) {
			DataResult<T> result = this.codec.Decode(json);
			return result.IsSuccess() ? result : DataResult<T>.Success(this.fallback);
		}
	}
}
