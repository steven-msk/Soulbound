namespace SoulboundEngine.Serialization {
	using Newtonsoft.Json.Linq;
	using System;

	public sealed record DirectCodec<T>(Func<T, JToken> encoder, Func<JToken, DataResult<T>> decoder) : Codec<T> {
			public override DataResult<T> Decode(JToken json) => this.decoder(json);

			public override JToken Encode(T value) => this.encoder(value);
		}
}
