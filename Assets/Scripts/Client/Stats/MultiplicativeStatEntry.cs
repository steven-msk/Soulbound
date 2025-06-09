using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MultiplicativeStatEntry<TProcessor, TValue> : AbstractStatEntry<TValue> where TProcessor : IMultiplicativeStatProcessor<TValue>, new() {
	private readonly List<TValue> flatBonuses = new();
	private readonly List<float> percentageBonuses = new();
	private readonly TProcessor processor = new();

	public MultiplicativeStatEntry(TValue baseValue) : base(baseValue) {
	}

	public void AddFlatBonus(TValue value) => flatBonuses.Add(value);

	public void AddPercentageBonus(float value) => percentageBonuses.Add(value);

	public float GetValue() => processor.CalculateFinalValue(BaseValue, flatBonuses, percentageBonuses);
}
