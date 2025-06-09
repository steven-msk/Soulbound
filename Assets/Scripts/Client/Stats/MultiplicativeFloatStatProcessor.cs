using System.Collections.Generic;
using System.Linq;

public class MultiplicativeFloatStatProcessor : IMultiplicativeStatProcessor<float> {
	public float CalculateFinalValue(float baseValue, IEnumerable<float> flatBonuses, IEnumerable<float> percentageBonuses) {
		return (baseValue + flatBonuses.Sum()) * (1 + percentageBonuses.Sum());
	}
}
