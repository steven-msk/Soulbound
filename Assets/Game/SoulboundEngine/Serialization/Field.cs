namespace SoulboundEngine.Serialization {
	using Newtonsoft.Json.Linq;
	using SoulboundEngine.Common.Patterns;
	using System;

#nullable enable

	public record Field {
		public static Field<O, V> Required<O, V>(string name, Codec<V> codec, Func<O, V> valueSupplier) {
			return new Field<O, V>(name, codec, valueSupplier, Optional<V>.Empty());
		}

		public static Field<O, V> Optional<O, V>(string name, Codec<V> codec, Func<O, V> valueSupplier, V fallback) {
			return new Field<O, V>(name, codec, valueSupplier, Optional<V>.Of(fallback));
		}
	}

	public record Field<O, V>(string name, Codec<V> codec, Func<O, V> valueSupplier, Optional<V> defaultValue) {
		public DataResult<V> DecodeFrom(JToken json) {
			JToken? value = json[this.name];
			if (value == null) {
				return this.defaultValue.IsPresent()
					? DataResult<V>.Success(this.defaultValue.GetValue())
					: DataResult<V>.Error($"Missing field '{this.name}'");
			}
			DataResult<V> result = this.codec.Decode(value);
			return this.defaultValue.IsPresent()
				? DataResult<V>.Success(result.ResultOrPartial().OrElse(this.defaultValue.GetValue()))
				: result;
		}
	}
}
