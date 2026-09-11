namespace SoulboundEngine.World.Gen.Biome {
	using System;
	using System.Collections.Generic;

	public sealed class Climate {
		private const float QUANTIZATION_FACTOR = 10000f;

		public static TargetPoint Target(float[] values) {
			long[] quantized = new long[values.Length];
			for (int i = 0; i < values.Length; i++) {
				quantized[i] = QuantizeCoord(values[i]);
			}
			return new TargetPoint(quantized);
		}

		public static ParameterPoint Parameters(float[] values, float offset) {
			Parameter[] points = new Parameter[values.Length];
			for (int i = 0; i < values.Length; i++) {
				points[i] = Parameter.Point(values[i]);
			}
			return new ParameterPoint(points, QuantizeCoord(offset));
		}

		public static T[] MapParameters<T>(Func<ParameterType, T> valueFunction) {
			return ParameterType.Map(valueFunction);
		}

		public static long QuantizeCoord(float coord) {
			return (long)(coord * QUANTIZATION_FACTOR);
		}

		public static float UnquantizeCoord(long coord) {
			return (float)(coord / QUANTIZATION_FACTOR);
		}

		public static Sampler Empty() {
			IDensityFunction zero = DensityFunctions.Zero();
			return new Sampler(ParameterType.Map(_ => zero));
		}

		public record TargetPoint(long[] values);

		public readonly struct ParameterType {
			public static readonly ParameterType SHAPE = new(0);
			public static readonly ParameterType[] VALUES = new[] {
				SHAPE
			};
			public readonly int index;

			public ParameterType(int index) {
				this.index = index;
			}

			public static T[] Map<T>(Func<ParameterType, T> valueFunction) {
				T[] result = new T[VALUES.Length];
				for (int i = 0; i < VALUES.Length; i++) {
					result[i] = valueFunction(VALUES[i]);
				}
				return result;
			}
		}

		public sealed record Parameter(long min, long max) {
			public static Parameter Point(float value) => Span(value, value);

			public static Parameter Span(float min, float max) {
				return min > max ? throw new ArgumentException($"min > max ({min} > {max})") 
					: new Parameter(QuantizeCoord(min), QuantizeCoord(max));
			}

			public long Distance(long target) {
				long above = target - this.max;
				long below = this.min - target;
				return above > 0L ? above : Math.Max(below, 0L);
			}
		}

		public sealed record ParameterPoint(Parameter[] parameters, long offset) {
			public long Fitness(TargetPoint targetPoint) {
				long sum = 0;
				for (int i = 0; i < this.parameters.Length; i++) {
					long d = this.parameters[i].Distance(targetPoint.values[i]);
					sum += d * d;
				}
				sum += this.offset * this.offset;
				return sum;
			}
		}

		public sealed record ParameterList<T>(List<(ParameterPoint point, T value)> entries) {

			// if there profiling says this is hot, bring the implementation to spatial partitioning
			public T FindBruteForce(TargetPoint target) {
				T best = default;
				long bestFitness = long.MaxValue;
				foreach ((ParameterPoint point, T value) in this.entries) {
					long fitness = point.Fitness(target);
					if (fitness < bestFitness) {
						bestFitness = fitness;
						best = value;
					}
				}
				return best;
			}
		}

		public sealed record Sampler(IDensityFunction[] densityFunctions) {
			public TargetPoint Sample(int x, int y) {
				IDensityFunction.SinglePointContext context = new(x, y);
				return Target(ParameterType.Map(p => (float)this.densityFunctions[p.index].Compute(context)));
			}
		}
	}
}
