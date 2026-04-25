using SoulboundEngine.Core.States;
using System.Collections.Generic;
using System.Linq;

namespace SoulboundEngine.Core.State {
	public class IntProperty : Property<int> {
		private readonly int[] values;

		private IntProperty(string name, int[] values) 
			: base(name, typeof(int)) {
			this.values = values;
		}

		public static IntProperty OfRange(string name, int minIncluded, int maxIncluded) {
			int[] values = new int[maxIncluded - minIncluded + 1];
			int current = minIncluded;

			while (current <= maxIncluded) {
				values[current] = current;
				current++;
			}

			return OfList(name, values);
		}

		public static IntProperty OfList(string name, int[] values) {
			return new IntProperty(name, values);
		}

		public override IEnumerable<object> GetValues() => this.values.Cast<object>();

		public override string Name(int value) {
			return value.ToString();
		}

		public override bool TryParse(string name, out int value) {
			return int.TryParse(name, out value);
		}
	}
}
