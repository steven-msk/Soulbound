using System;
using System.Collections.Generic;

namespace SoulboundBackend.Client.Stats {
	public interface IStatProcessor<TValue> where TValue : struct, IComparable<TValue> {
		public TValue ProcessFinalValue(TValue baseValue, IEnumerable<ValueModifier<TValue>> modifiers);
	}
}
