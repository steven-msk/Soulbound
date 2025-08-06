using System.Collections.Generic;
using System.Linq;

public class FlatIntStatProcessor : IStatProcessor<int> {
	public int ProcessFinalValue(int baseValue, IEnumerable<int> flatBonuses) {
		return baseValue + flatBonuses.Sum();
	}
}
