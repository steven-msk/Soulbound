namespace SoulboundEngine.Serialization {
	using Newtonsoft.Json.Linq;
	using System;

#nullable enable

	public record Field<O, V>(string name, Codec<V> codec, Func<O, V> valueSupplier) {
		public static Field<O, V> Of(string name, Codec<V> codec, Func<O, V> valueSupplier) {
			return new Field<O, V>(name, codec, valueSupplier);
		}

		public DataResult<V> DecodeFrom(JToken json) {
			JToken? token = json[this.name];
			return token == null ? DataResult<V>.Error($"Missing field '{this.name}") : this.codec.Decode(token);
		}
	}
}
