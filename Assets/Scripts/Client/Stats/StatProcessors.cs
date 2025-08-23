using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class StatProcessors {
	private sealed class MultiplicativeStatProcessor<TValue> : IMultiplicativeStatProcessor<TValue> where TValue : struct, IComparable<TValue> {
		private readonly Func<TValue, TValue, TValue> flatAdder;
		private readonly Func<TValue, float, float> productResolver;

		internal MultiplicativeStatProcessor(Func<TValue, TValue, TValue> flatAdder, Func<TValue, float, float> productResolver) {
			this.flatAdder = flatAdder;
			this.productResolver = productResolver;
		}

		public float ProcessFinalValue(TValue baseValue, IEnumerable<TValue> flatBonuses, IEnumerable<float> percentageBonuses) {
			TValue flatAddition = StatProcessors.Add(baseValue, flatBonuses, flatAdder);
			float percentageAddition = 1 + percentageBonuses.Sum();
			return productResolver.Invoke(flatAddition, percentageAddition);
		}
	}

	private sealed class FlatStatProcessor<TValue> : IStatProcessor<TValue> where TValue : struct, IComparable<TValue> {
		private readonly Func<TValue, TValue, TValue> adder;

		internal FlatStatProcessor(Func<TValue, TValue, TValue> adder) {
			this.adder = adder;
		}

		public TValue ProcessFinalValue(TValue baseValue, IEnumerable<TValue> flatBonuses) { 
			return StatProcessors.Add(baseValue, flatBonuses, adder); 
		}
	}

	private sealed class PercentageStatProcessor : IStatProcessor<float> {
		public float ProcessFinalValue(float baseValue, IEnumerable<float> percentageBonuses) {
			return baseValue * (1 + percentageBonuses.Sum());
		}
	}

	private static TValue Add<TValue>(TValue baseValue, IEnumerable<TValue> values, Func<TValue, TValue, TValue> adder) {
		TValue result = baseValue;
		foreach (var value in values) {
			result = adder.Invoke(result, value);
		}
		return result;
	}

	public static IStatProcessor<int> FlatInt() => new FlatStatProcessor<int>((a, b) => a + b);

	public static IStatProcessor<float> FlatFloat() => new FlatStatProcessor<float>((a, b) => a + b);

	public static IStatProcessor<float> Percentage() => new PercentageStatProcessor();

	public static IMultiplicativeStatProcessor<int> MultiplicativeInt() {
		return new MultiplicativeStatProcessor<int>((a, b) => a + b, (a, b) => a * b);
	}

	public static IMultiplicativeStatProcessor<float> MultiplicativeFloat() {
		return new MultiplicativeStatProcessor<float>((a, b) => a + b, (a, b) => a * b);
	}

	public static IMultiplicativeStatProcessor<TValue> Multiplicative<TValue>() where TValue : struct, IComparable<TValue> {
		return default(TValue) switch {
			int => (IMultiplicativeStatProcessor<TValue>)MultiplicativeInt(),
			float => (IMultiplicativeStatProcessor<TValue>)MultiplicativeFloat(),
			_ => throw new NotSupportedException($"Unsuppoted stat value type {typeof(TValue)}")
		};
	}
}
