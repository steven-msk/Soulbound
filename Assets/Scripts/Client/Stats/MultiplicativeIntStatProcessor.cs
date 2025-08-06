using System.Collections.Generic;
using System.Linq;

public class MultiplicativeIntStatProcessor : IMultiplicativeStatProcessor<int> {

	public float ProcessFinalValue(int baseValue, IEnumerable<int> flatBonuses, IEnumerable<float> percentageBonuses) {
		return (baseValue + flatBonuses.Sum()) * (1 + percentageBonuses.Sum());
	}
}
