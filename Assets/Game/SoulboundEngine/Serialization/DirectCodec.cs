namespace SoulboundEngine.Serialization {
	using Newtonsoft.Json.Linq;
	using System;

	public sealed record DirectCodec<T>(Func<T, JToken> encoder, Func<JToken, int, DataResult<T>> decoder) : Codec<T> {
			public override DataResult<T> Decode(JToken json, int sinceVersion) => this.decoder(json, sinceVersion);

			public override JToken Encode(T value) => this.encoder(value);
		}
}
