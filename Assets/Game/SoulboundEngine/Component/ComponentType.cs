namespace SoulboundEngine.Component {
	using SoulboundEngine.Registry;
	using SoulboundEngine.Serialization;
	using System;
	using System.Collections.Generic;

#nullable enable

	public abstract class ComponentType {
		public static readonly Codec<ComponentType> CODEC = Identifier.CODEC.FlatXmap(
			encode: c => c.key.value,
			decode: i => Registries.COMPONENT_TYPE.GetEntry(i) is { } entry
				? DataResult<ComponentType>.Success(entry.GetValue())
				: DataResult<ComponentType>.Error($"Invalid component: {i}")
		);
		public static readonly Dictionary<Identifier, ComponentType> FROM_ID = new();
		private readonly RegistryKey<ComponentType> key;

		protected ComponentType(RegistryKey<ComponentType> key) {
			this.key = key;
			FROM_ID.Add(key.value, this);
		}

		public abstract bool IsTransient();

		public abstract Codec<object> GetBoxedValueCodec();

		public RegistryKey<ComponentType> GetKey() => this.key;

		public static ComponentType? Get(Identifier id) {
			return FROM_ID.GetValueOrDefault(id);
		}

		public static Identifier GetId(ComponentType type) {
			return type.key.value;
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

		public override Codec<object> GetBoxedValueCodec() {
			return this.CodecOrThrow().XmapToObject();
		}

		public Codec<T> CodecOrThrow() {
			return this.CodecOrThrow(new NotSupportedException($"Cannot serialize transient component '{GetId(this)}'"));
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
