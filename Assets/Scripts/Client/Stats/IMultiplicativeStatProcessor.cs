using System.Collections.Generic;

public interface IMultiplicativeStatProcessor<TValue> {
	public float CalculateFinalValue(TValue baseValue, IEnumerable<TValue> flatBonuses, IEnumerable<float> percentageBonuses);
}
