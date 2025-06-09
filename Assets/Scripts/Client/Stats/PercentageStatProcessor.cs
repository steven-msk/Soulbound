using System.Collections.Generic;
using System.Linq;

public class PercentageStatProcessor : IStatProcessor<float> {
	public float CalculateFinalValue(float baseValue, IEnumerable<float> percentageBonuses) {
		return baseValue * (1 + percentageBonuses.Sum());
	}
}
