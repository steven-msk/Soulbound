namespace SoulboundEngine.World.Gen.Biome {
	public class DensityFunctions {
		public record Moise(IDensityFunction.NoiseEntry noise) : IDensityFunction {
			public double Compute(IDensityFunction.IContext context) {
				return this.noise.Get(context.blockX, context.blockY, context.blockZ);
			}
		}
	}
}
