namespace SoulboundEngine.World.Gen.Biome {
	using System;

	public sealed class Climate {
		public static TargetPoint Target(double[] values) {
			return new TargetPoint(values);
		}

		public record TargetPoint(double[] values);

		public readonly struct ParameterType {
			public static readonly ParameterType SHAPE = new(0);
			public static readonly ParameterType[] VALUES = new[] {
				SHAPE
			};
			public readonly int index;

			public ParameterType(int index) {
				this.index = index;
			}

			public static double[] Map(Func<ParameterType, double> valueFunction) {
				double[] result = new double[VALUES.Length];
				for (int i = 0; i < VALUES.Length; i++) {
					result[i] = valueFunction(VALUES[i]);
				}
				return result;
			}
		}

		public sealed record Sampler(IDensityFunction[] densityFunctions) {
			public TargetPoint Sample(int x, int y) {
				IDensityFunction.SinglePointContext context = new(x, y);
				return Target(ParameterType.Map(p => this.densityFunctions[p.index].Compute(context)));
			}
		}
	}
}
