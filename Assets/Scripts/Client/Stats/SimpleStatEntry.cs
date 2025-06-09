using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SimpleStatEntry<TProcessor, TValue> : AbstractStatEntry<TValue> where TProcessor : IStatProcessor<TValue>, new() {
	private readonly List<TValue> bonuses = new();
	private readonly TProcessor processor = new();

	public SimpleStatEntry(TValue baseValue) : base(baseValue) {
	}

	public void AddBonus(TValue value) => bonuses.Add(value);

	public TValue GetValue() => processor.CalculateFinalValue(BaseValue, bonuses);
}
