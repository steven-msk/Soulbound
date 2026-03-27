using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.Stats {
	public sealed class ValueModificationContext<TValue> : IModificationContext<TValue> 
			where TValue : struct, IComparable<TValue> {
		private readonly ValueModifier<TValue> valueModifier;
		private readonly StatEntry<TValue> entry;

		public ValueModificationContext(ValueModifier<TValue> valueModifier, StatEntry<TValue> entry) {
			this.valueModifier = valueModifier;
			this.entry = entry;
		}

		public bool IsValid() {
			return entry.definition.supportedApplications.Supports(valueModifier.applicationType);
		}
	}
}
