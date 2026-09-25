namespace SoulboundEngine.Serialization {
	using Newtonsoft.Json.Linq;
	using System;

	public abstract record MapCodec<T> {
		public abstract DataResult<T> Decode(JObject json, int sinceVersion);
		public abstract void Encode(T value, JObject json);

		public static MapCodec<T> Of(Func<JObject, int, DataResult<T>> decode, Action<T, JObject> encode) {
			return new Impl(decode, encode);
		}

		public Codec<T> AsCodec() => Codec<T>.Of(
			encode: value => {
				JObject obj = new();
				this.Encode(value, obj);
				return obj;
			},
			decode: (json, sinceVersion) => json is JObject obj
				? this.Decode(obj, sinceVersion)
				: DataResult<T>.Error($"Expected json object, got {json.Type}")
		);

		public MapCodec<U> Xmap<U>(Func<T, U> to, Func<U, T> from) {
			return MapCodec<U>.Of(
				encode: (u, target) => this.Encode(from(u), target),
				decode: (obj, sinceVersion) => this.Decode(obj, sinceVersion).Map<U>(to)
			);
		}

		private sealed record Impl(Func<JObject, int, DataResult<T>> decode, Action<T, JObject> encode) : MapCodec<T> {
			public override DataResult<T> Decode(JObject json, int sinceVersion) => this.decode(json, sinceVersion);

			public override void Encode(T value, JObject json) => this.encode(value, json);
		}
	}
}
