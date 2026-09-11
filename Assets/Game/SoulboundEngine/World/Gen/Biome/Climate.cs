namespace SoulboundEngine.World.Gen.Biome {
	using System;

	public sealed class Climate {
		public static TargetPoint Target(float[] values) {
			return new TargetPoint(values);
		}

		public record TargetPoint(float[] values);

		public readonly struct Parameter {
			public static readonly Parameter SHAPE = new(0);
			public static readonly Parameter[] VALUES = new[] {
				SHAPE
			};
			public readonly int index;

			public Parameter(int index) {
				this.index = index;
			}

			public static float[] Map(Func<Parameter, float> valueFunction) {
				float[] result = new float[VALUES.Length];
				for (int i = 0; i < VALUES.Length; i++) {
					result[i] = valueFunction(VALUES[i]);
				}
				return result;
			}
		}

		public sealed record Sampler(IDensityFunction[] densityFunctions) {
			public TargetPoint Sample(int x, int y) {
				IDensityFunction.SinglePointContext context = new(x, y);
				return new TargetPoint(Parameter.Map(p => this.densityFunctions[p.index].Compute(context)));
			}
		}
	}
}
