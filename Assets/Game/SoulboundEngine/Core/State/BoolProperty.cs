using System.Collections.Generic;

namespace SoulboundEngine.Core.States {
	public sealed class BoolProperty : Property<bool> {
		private BoolProperty(string name)
			: base(name, typeof(bool)) {
		}

		public static BoolProperty Of(string name) => new(name);

		public override IEnumerable<object> GetValues() {
			return new object[] { true, false };
		}

		public override string Name(bool value) {
			return value.ToString();
		}

		public override bool TryParse(string name, out bool value) {
			return bool.TryParse(name, out value);
		}
	}
}
