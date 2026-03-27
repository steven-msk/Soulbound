
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoulboundEngine.Client.Stats {
	public class StatProcessor<TValue> : IStatProcessor<TValue> where TValue : struct, IComparable<TValue> {

		public TValue ProcessFinalValue(
				TValue baseValue,
				StatEntry<TValue> entry,
				IEnumerable<IStatEntryModifier<TValue>> modifiers,
				Dictionary<IStatEntryModifier<TValue>, IModificationProcedure<TValue>> procedures
		) {
			TValue result = baseValue;
			foreach (var modifier in modifiers) {
				result = procedures[modifier].Apply(result, modifier, entry);
			}
			return result;
		}

	}
}
