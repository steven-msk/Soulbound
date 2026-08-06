using System;

namespace SoulboundEngine.Client.Component {
	public record Component {
		// Sentinel marking "this type is removed relative to baseMap", distinct from
		// simply not having an entry in changedComponents (which means "defer to baseMap").
		public static readonly object REMOVED = new();

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
