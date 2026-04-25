using System;
using System.Collections.Generic;

#nullable enable

namespace SoulboundEngine.Core.States {
	public abstract class Property {
		public abstract string name { get; }
		public abstract Type type { get; }

		public override string ToString() {
			return $"property[name={this.name}, type={this.type}]";
		}

		public override int GetHashCode() {
			return HashCode.Combine(this.name, this.type);
		}

		public abstract IEnumerable<object> GetValues();
	}

	public abstract class Property<T> : Property where T : IComparable<T> {
		public override string name { get; }
		public override Type type { get; }
		private int hashCodeCache;

		public Property(string name, Type type) {
			this.name = name;
			this.type = type;
		}

		public int ComputeHashCode() {
			if (this.hashCodeCache == 0) {
				this.hashCodeCache = this.GetHashCode();
			}
			return this.hashCodeCache;
		}

		public abstract string Name(T value);
		public abstract bool TryParse(string name, out T value);

		public Value CreateValue<O, S>(State<O, S> state) {
			return this.CreateValue(state.Get(this));
		}

		public Value CreateValue(T value) {
			return new Value(this, value);
		}

		public override string ToString() {
			return base.ToString();
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public override bool Equals(object obj) {
			return obj is Property<T> prop
				&& prop.name == this.name
				&& prop.type == this.type;
		}

		public sealed class Value {
			public Property<T> property { get; }
			public T value { get; }

			public Value(Property<T> property, T value) {
				this.property = property;
				this.value = value;
			}

			public override string ToString() {
				return $"propety_value[property={this.property}, value={this.value}]";
			}

			public override int GetHashCode() {
				return HashCode.Combine(this.property.ComputeHashCode(), this.value);
			}

			public override bool Equals(object obj) {
				return obj is Value val
					&& val.property.Equals(this.property)
					&& val.value.Equals(this.value);
			}
		}
	}
}
