namespace SoulboundEngine.World.Gen.Biome {
	public class DensityFunctions {
		public static IDensityFunction Zero() => Constant.ZERO;

		public record Noise(IDensityFunction.NoiseEntry noise) : IDensityFunction {
			public double Compute(IDensityFunction.IContext context) {
				return this.noise.Get(context.blockX, context.blockY, 0.0d);
			}
		}

		public record Constant(double value) : IDensityFunction {
			public static readonly Constant ZERO = new(0.0d);

			public double Compute(IDensityFunction.IContext context) {
				return this.value;
			}
		}
	}
}
