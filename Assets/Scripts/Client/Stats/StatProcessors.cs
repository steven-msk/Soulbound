using System;
using System.Collections.Generic;
using System.Linq;

namespace SoulboundBackend.Client.Stats {
	public static class StatProcessors {
		private sealed class MultiplicativeStatProcessor<TValue> : IStatProcessor<TValue> where TValue : struct, IComparable<TValue> {
			private readonly Func<TValue, TValue, TValue> adder;
			private readonly Func<TValue, float, TValue> productResolver;

			internal MultiplicativeStatProcessor(Func<TValue, TValue, TValue> flatAdder, Func<TValue, float, TValue> productResolver) {
				this.adder = flatAdder;
				this.productResolver = productResolver;
			}

			public TValue ProcessFinalValue(TValue baseValue, IEnumerable<ValueModifier<TValue>> modifiers) {
				var flatModifiers = modifiers.Where(m => m.applicationType == StatApplicationType.Flat);
				var percentageModifiers = modifiers.Where(m => m.applicationType == StatApplicationType.Percentage)
					.Cast<ValueModifier<float>>();       // Safe cast because percentage stats are always floats
				TValue flatAddition = StatProcessors.Add(baseValue, flatModifiers.Select(m => m.value), adder);
				float percentageAddition = 1 + percentageModifiers.Sum(m => m.value);
				return productResolver.Invoke(flatAddition, percentageAddition);
			}
		}

		private sealed class FlatStatProcessor<TValue> : IStatProcessor<TValue> where TValue : struct, IComparable<TValue> {
			private readonly Func<TValue, TValue, TValue> adder;

			internal FlatStatProcessor(Func<TValue, TValue, TValue> adder) {
				this.adder = adder;
			}

			public TValue ProcessFinalValue(TValue baseValue, IEnumerable<ValueModifier<TValue>> modifiers) {
				return StatProcessors.Add(baseValue, modifiers.Select(m => m.value), adder);
			}
		}

		private sealed class PercentageStatProcessor : IStatProcessor<float> {
			public float ProcessFinalValue(float baseValue, IEnumerable<ValueModifier<float>> modifiers) {
				return baseValue * (1 + modifiers.Sum(m => m.value));
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

		public static IStatProcessor<TValue> Flat<TValue>() where TValue : struct, IComparable<TValue> {
			return default(TValue) switch {
				int => (IStatProcessor<TValue>)FlatInt(),
				float => (IStatProcessor<TValue>)FlatFloat(),
				_ => throw new NotSupportedException($"Unsuppoted flat stat processor type {typeof(TValue)}")
			};
		}

		public static IStatProcessor<float> Percentage() => new PercentageStatProcessor();

		public static IStatProcessor<int> MultiplicativeInt() {
			return new MultiplicativeStatProcessor<int>((a, b) => a + b, (a, b) => (int)(a * b));
		}

		public static IStatProcessor<float> MultiplicativeFloat() {
			return new MultiplicativeStatProcessor<float>((a, b) => a + b, (a, b) => a * b);
		}

		public static IStatProcessor<TValue> Multiplicative<TValue>() where TValue : struct, IComparable<TValue> {
			return default(TValue) switch {
				int => (IStatProcessor<TValue>)MultiplicativeInt(),
				float => (IStatProcessor<TValue>)MultiplicativeFloat(),
				_ => throw new NotSupportedException($"Unsuppoted multiplicative stat processor type {typeof(TValue)}")
			};
		}
	}
}
