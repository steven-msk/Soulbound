namespace SoulboundEngine.Component {
	using Newtonsoft.Json.Linq;
	using SoulboundEngine.Registry;
	using System;
	using System.Collections.Generic;

#nullable enable

	public abstract class ComponentType {
		public static readonly Dictionary<Identifier, ComponentType> FROM_ID = new();
		private readonly RegistryKey<ComponentType> key;

		protected ComponentType(RegistryKey<ComponentType> key) {
			this.key = key;
			FROM_ID.Add(key.value, this);
		}

		public abstract bool IsTransient();

		public abstract JToken ToJson(object value);

		public abstract object FromJson(JToken token);

		public RegistryKey<ComponentType> GetKey() => this.key;

		public static ComponentType? Get(Identifier id) {
			return FROM_ID.GetValueOrDefault(id);
		}

		public static Identifier GetId(ComponentType type) {
			return type.key.value;
		}

		public sealed record Codec<T>(Func<T, JToken> encoder, Func<JToken, T> decoder) {
			public JToken Encode(T value) => this.encoder(value);

			public T Decode(JToken token) => this.decoder(token);
		}
	}

	public class ComponentType<T> : ComponentType {
		private readonly Codec<T>? codec;

		private ComponentType(RegistryKey<ComponentType> key, Codec<T>? codec) 
			: base(key) {
			this.codec = codec;
		}

		public Codec<T>? Codec => this.codec;

		public Codec<T> CodecOrThrow(Exception e) {
			return this.codec ?? throw e;
		}

		public override bool IsTransient() {
			return this.codec == null;
		}

		public override JToken ToJson(object value) {
			if (value is not T typed) throw new InvalidCastException($"Component value {value} cannot be cast to {typeof(T)}");
			return this.CodecOrThrow(new NotSupportedException($"Cannot serialize transient component '{GetId(this)}'")).Encode(typed);
		}

		public override object FromJson(JToken token) {
			return this.CodecOrThrow(new NotSupportedException($"Cannot serialize transient component '{GetId(this)}'")).Decode(token);
		}

		public static Builder Create(RegistryKey<ComponentType> key) => new(key);

		public sealed class Builder {
			private readonly RegistryKey<ComponentType> key;
			private Codec<T>? codec;

			public Builder(RegistryKey<ComponentType> key) {
				this.key = key;
			}

			public Builder Codec(Codec<T> codec) {
				this.codec = codec;
				return this;
			}

			public ComponentType<T> Build() {
				return new ComponentType<T>(this.key, this.codec);
			}
		}
	}
}
