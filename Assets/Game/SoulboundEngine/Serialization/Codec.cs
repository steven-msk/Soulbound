namespace SoulboundEngine.Serialization {
	using Newtonsoft.Json.Linq;

	public abstract record Codec<T> {
		public abstract JToken Encode(T value);
		public abstract DataResult<T> Decode(JToken json);

		public Codec<T> WithDefault(T fallback) => new DefaultingCodec<T>(this, fallback);
	}
}
