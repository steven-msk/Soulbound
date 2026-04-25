using System;
using System.Collections.Generic;
using System.Linq;

namespace SoulboundEngine.Core.States {
	public sealed class EnumProperty<E> : Property<E> where E : struct, Enum {
		private readonly E[] values;

		private EnumProperty(string name, Func<E[]> valueSupplier) 
			: base(name, typeof(E)) {
			this.values = valueSupplier();
		}

		public static EnumProperty<E> Of(string name) {
			return new EnumProperty<E>(name, Enum.GetValues(typeof(E)).Cast<E>().ToArray);
		}

		public static EnumProperty<E> Of(string name, params E[] values) {
			return new EnumProperty<E>(name, () => values);
		}

		public static EnumProperty<E> Of(string name, Func<E[]> valueSupplier) {
			return new EnumProperty<E>(name, valueSupplier);
		}

		public override IEnumerable<object> GetValues() {
			return this.values.Cast<object>();
		}

		public override string Name(E value) {
			return value.ToString();
		}

		public override bool TryParse(string name, out E value) {
			return Enum.TryParse(name, out value);
		}
	}
}
