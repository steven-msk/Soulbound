using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class FlatFloatStatProcessor : IStatProcessor<float> {
	public float CalculateFinalValue(float baseValue, IEnumerable<float> flatBonuses) {
		return baseValue + flatBonuses.Sum();
	}
}