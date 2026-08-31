namespace SoulboundEngine.Component {
	using Newtonsoft.Json.Linq;
	using SoulboundEngine.Serialization;
	using System;

	public record Component {
		// Sentinel marking "this component is removed relative to baseMap", not the same as
		// not having an entry in changedComponents, which means "fallback to baseMap".
		public static readonly object REMOVED = new();
		public static bool IsRemoved(object obj) => ReferenceEquals(obj, REMOVED);

		public readonly ComponentType boxedType;
		public readonly object boxedValue;

		protected Component(ComponentType boxedType, object boxedValue) {
			this.boxedType = boxedType;
			this.boxedValue = boxedValue;
		}

		public static Component<T> Of<T, V>(ComponentType<T> type, V value) where V : T {
			return new Component<T>(type, value);
		}

		public static Component<T> Of<T>(ComponentType<T> type, object value) {
			return new Component<T>(type, (T)value);
		}

		public static Component Of(ComponentType type, object value) => new(type, value);

		public static Codec<Component> MakeCodec(ComponentType componentType) => MakeCodec(componentType.GetBoxedValueCodec());

		public static Codec<Component> MakeCodec(Codec<object> boxedValueCodec) => RecordCodec<Component, ComponentType, object>.Of(
			ComponentTypeField(),
			Field.Required<Component, object>("value", WithRemovedSentinelCodec(boxedValueCodec), c => c.boxedValue),
			(type, value) => new Component(type, value)
		);

		public static Codec<object> WithRemovedSentinelCodec(Codec<object> boxedValueCodec) => Codec<object>.Of(
			encode: o => IsRemoved(o) ? JValue.CreateNull() : boxedValueCodec.Encode(o),
			decode: json => json.Type == JTokenType.Null ? DataResult<object>.Success(REMOVED) : boxedValueCodec.Decode(json)
		);

		public static DataResult<ComponentType> GetTypeFrom(JToken token) {
			return ComponentTypeField().DecodeFrom(token);
		} 

		private static Field<Component, ComponentType> ComponentTypeField() {
			return Field.Required<Component, ComponentType>("component", ComponentType.CODEC, c => c.boxedType);
		}

		public override string ToString() {
			return $"component[type={this.boxedType},value={this.boxedValue}]";
		}

		public virtual bool Equals(Component other) {
			return other.boxedType.Equals(this.boxedType) && other.boxedValue.Equals(this.boxedValue);
		}

		public override int GetHashCode() {
			return HashCode.Combine(this.boxedType, this.boxedValue);
		}
	}

	public sealed record Component<T>(ComponentType<T> type, T value) : Component(type, value), IEquatable<Component<T>> {
		public override string ToString() {
			return $"component[type={this.type},value={this.value}]";
		}

		public bool Equals(Component<T> other) {
			return other.type.Equals(this.type) && other.value.Equals(this.value);
		}

		public override int GetHashCode() {
			return HashCode.Combine(this.type, this.value);
		}
	}
}
