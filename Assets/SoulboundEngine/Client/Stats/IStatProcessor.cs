using System;
using System.Collections.Generic;

namespace SoulboundEngine.Client.Stats {
	public interface IStatProcessor<TValue> where TValue : struct, IComparable<TValue> {
		public TValue ProcessFinalValue(
			TValue baseValue,
			StatEntry<TValue> entry,
			IEnumerable<IStatEntryModifier<TValue>> modifiers,
			Dictionary<IStatEntryModifier<TValue>, IModificationProcedure<TValue>> procedures
		);
	}
}
