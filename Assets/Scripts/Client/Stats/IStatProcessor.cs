using System;
using System.Collections.Generic;

public interface IStatProcessor<TValue> where TValue : struct, IComparable<TValue> {
	public TValue ProcessFinalValue(TValue baseValue, IEnumerable<SerializableStat<TValue>> modifiers);
}
