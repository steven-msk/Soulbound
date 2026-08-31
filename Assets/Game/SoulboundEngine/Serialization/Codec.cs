namespace SoulboundEngine.Serialization {
	using Newtonsoft.Json.Linq;
	using SoulboundEngine.Common;
	using System;
	using System.Collections.Generic;

	public abstract record Codec<T> {
		public abstract JToken Encode(T value);
		public abstract DataResult<T> Decode(JToken json);

		public static Codec<T> Of(Func<T, JToken> encode, Func<JToken, DataResult<T>> decode) {
			return new Impl(encode, decode);
		}

		public Codec<T> WithDefault(T fallback) => new DefaultingCodec<T>(this, fallback);

		public Codec<object> XmapToObject() => this.Xmap<object>(v => v, o => (T)o);

		public Codec<List<T>> ListOf() => new ListCodec<T>(this);

		public Codec<Optional<T>> MakeOptional() => Codec<Optional<T>>.Of(
			encode: v => v.IsEmpty() ? JValue.CreateNull() : this.Encode(v.GetValue()),
			decode: json => json.Type == JTokenType.Null ? DataResult<Optional<T>>.Success(Optional<T>.Empty()) : this.Decode(json).Map(Optional<T>.Of)
		);

		public Codec<U> Xmap<U>(Func<T, U> to, Func<U, T> from) {
			return Codec<U>.Of(
				encode: u => this.Encode(from(u)),
				decode: json => this.Decode(json).Map<U>(to)
			);
		}

		public Codec<U> FlatXmap<U>(Func<T, DataResult<U>> decode, Func<U, T> encode) {
			return Codec<U>.Of(
				encode: u => this.Encode(encode(u)),
				decode: json => this.Decode(json).FlatMap(decode)
			);
		}

		private sealed record Impl(Func<T, JToken> encode, Func<JToken, DataResult<T>> decode) : Codec<T> {
			public override DataResult<T> Decode(JToken json) => this.decode(json);

			public override JToken Encode(T value) => this.encode(value);
		}
	}
}
