using SoulboundBackend.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoulboundBackend.Client.Stats {
	public class StatProcessor<TValue> : IStatProcessor<TValue> where TValue : struct, IComparable<TValue> {
		private static readonly Logger logger = Logger.CreateInstance();

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
