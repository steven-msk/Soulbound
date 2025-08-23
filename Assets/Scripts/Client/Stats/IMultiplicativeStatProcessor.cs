using System;
using System.Collections.Generic;

public interface IMultiplicativeStatProcessor<TValue> where TValue : struct, IComparable<TValue> {
	public float ProcessFinalValue(TValue baseValue, IEnumerable<TValue> flatBonuses, IEnumerable<float> percentageBonuses);
}
