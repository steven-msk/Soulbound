using System.Collections.Generic;

public interface IMultiplicativeStatProcessor<TValue> {
	public float ProcessFinalValue(TValue baseValue, IEnumerable<TValue> flatBonuses, IEnumerable<float> percentageBonuses);
}
