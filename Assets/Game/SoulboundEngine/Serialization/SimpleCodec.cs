namespace SoulboundEngine.Serialization {
	using Newtonsoft.Json.Linq;
	using System;

	public record SimpleCodec<T>(Func<T, object> encoder, Func<JToken, DataResult<T>> decoder) : Codec<T> {

		public override DataResult<T> Decode(JToken json, int sinceVersion) => this.decoder(json);

		public override JToken Encode(T value) => new JValue(this.encoder(value));
	}
}
