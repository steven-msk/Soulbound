namespace SoulboundEngine.World.Gen.Biome {
	using SoulboundEngine.Registry;

#nullable enable

	public interface IDensityFunction {

		double Compute(IContext context);
		
		public interface IContext {
			int blockX { get; }
			int blockY { get; }
		}

		public sealed record SinglePointContext(int blockX, int blockY) : IContext;

		public sealed record NoiseEntry(RegistryEntry<NormalNoise.Parameters> parameters, NormalNoise? noise) {
			public double Get(double x, double y, double z) {
				return this.noise?.Get(x, y, z) ?? 0.0d;
			}
		}
	}
}
