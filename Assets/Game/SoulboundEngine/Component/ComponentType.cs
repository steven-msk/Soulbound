namespace SoulboundEngine.Component {
	using Newtonsoft.Json.Linq;
	using System;

#nullable enable

	public abstract class ComponentType {
		public sealed record Codec<T>(Func<T, JToken> encoder, Func<JToken, T> decoder) {
			public JToken Encode(T value) => this.encoder(value);

			public T Decode(JToken token) => this.decoder(token);
		}

		public abstract bool IsTransient();
	}

	public class ComponentType<T> : ComponentType {
		private readonly Codec<T>? codec;

		private ComponentType(Codec<T>? codec) {
			this.codec = codec;
		}

		public Codec<T>? Codec => this.codec;

		public Codec<T> CodecOrThrow(Exception e) {
			return this.codec ?? throw e;
		}

		public override bool IsTransient() {
			return this.codec == null;
		}

		public static Builder Create() => new();

		public sealed class Builder {
			private Codec<T>? codec;

			public Builder Codec(Codec<T> codec) {
				this.codec = codec;
				return this;
			}

			public ComponentType<T> Build() {
				return new ComponentType<T>(this.codec);
			}
		}
	}
}
