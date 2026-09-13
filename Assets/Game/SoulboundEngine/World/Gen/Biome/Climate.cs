namespace SoulboundEngine.World.Gen.Biome {
	using SoulboundEngine.Common;
	using SoulboundEngine.Registry;
	using SoulboundEngine.World.Gen.Function;
	using SoulboundEngine.World.Gen.Noise;
	using System;
	using System.Collections.Generic;
	using System.Linq;

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
			public static readonly ParameterType SHAPE = new(0, "shape", NoiseTypes.SHAPE);
			public static readonly ParameterType[] VALUES = new[] {
				SHAPE
			};
			public readonly RegistryKey<NormalNoise.Parameters> noiseKey;
			public readonly int index;
			public readonly string serializedName;

			public ParameterType(int index, string serializedName, RegistryKey<NormalNoise.Parameters> noiseKey) {
				this.index = index;
				this.serializedName = serializedName;
				this.noiseKey = noiseKey;
			}

			public static T[] Map<T>(Func<ParameterType, T> valueFunction) {
				T[] result = new T[VALUES.Length];
				for (int i = 0; i < VALUES.Length; i++) {
					result[i] = valueFunction(VALUES[i]);
				}
				return result;
			}

			public T Get<T>(T[] array) {
				return array[this.index];
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

			public override int GetHashCode() {
				return HashCode.Combine(this.min, this.max);
			}
		}

		public sealed record ParameterPoint(Parameter[] parameters, long offset) {
			public static ParameterPoint Create(Func<Builder, Builder> builder) {
				return builder(new Builder()).Build();
			}

			public long Fitness(TargetPoint targetPoint) {
				long sum = 0;
				for (int i = 0; i < this.parameters.Length; i++) {
					long d = this.parameters[i].Distance(targetPoint.values[i]);
					sum += d * d;
				}
				sum += this.offset * this.offset;
				return sum;
			}

			public sealed class Builder {
				private readonly Dictionary<ParameterType, Parameter> values = new();
				private long offset;

				public Builder Offset(long offset) {
					this.offset = offset;
					return this;
				}

				public Builder Point(ParameterType type, float value) {
					this.values.Add(type, Parameter.Point(value));
					return this;
				}

				public Builder Span(ParameterType type, float min, float max) {
					this.values.Add(type, Parameter.Span(min, max));
					return this;
				}

				public ParameterPoint Build() {
					foreach (ParameterType type in ParameterType.VALUES) {
						if (!this.values.ContainsKey(type)) {
							throw new NotSupportedException("Missing climate parameter: {}".WithArgs(type.serializedName));
						}
					}
					return new ParameterPoint(this.values.Values.ToArray(), this.offset);
				}
			}

			public override int GetHashCode() {
				return HashCode.Combine(this.parameters, this.offset);
			}
		}

		public sealed record ParameterList<T>(List<(ParameterPoint point, T value)> entries) {
			public static ParameterList<T> Create(Func<Builder, Builder> builder) {
				return builder(new Builder()).Build();
			}

			// if profiling says this is hot, bring the implementation to spatial partitioning
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

			public sealed class Builder {
				private readonly Dictionary<ParameterPoint, T> entries = new();

				public Builder Add(ParameterPoint point, T value) {
					this.entries.Add(point, value);
					return this;
				}

				public ParameterList<T> Build() {
					List<(ParameterPoint, T)> entries = new();
					foreach ((ParameterPoint point, T value) in this.entries) {
						entries.Add((point, value));
					}
					return new ParameterList<T>(entries);
				}
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
