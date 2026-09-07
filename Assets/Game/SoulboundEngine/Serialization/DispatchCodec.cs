namespace SoulboundEngine.Serialization {
	using Newtonsoft.Json.Linq;
	using System;

#nullable enable

	public sealed record DispatchCodec<T, K>(Codec<K> keyCodec, string typeField, Func<T, K> keyOf, Func<K, MapCodec<T>> codecOf) : Codec<T> {
		public override DataResult<T> Decode(JToken json) {
			if (json is not JObject obj) {
				return DataResult<T>.Error($"Expected json object, got {json.Type}");
			}
			JToken? typeJson = obj[this.typeField];
			return typeJson == null
				? DataResult<T>.Error($"Missing type field '{this.typeField}'")
				: this.keyCodec.Decode(typeJson).FlatMap(key => this.codecOf(key).Decode(obj));
		}

		public override JToken Encode(T value) {
			K key = this.keyOf(value);
			JObject obj = new() {
				[this.typeField] = this.keyCodec.Encode(key)
			};
			this.codecOf(key).Encode(value, obj);
			return obj;
		}
	}
}
