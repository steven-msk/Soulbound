namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.Registry;

	public static class NoiseData {
		public static NormalNoise.Parameters Init(Registry<NormalNoise.Parameters> registry) {
			return Registry<NormalNoise.Parameters>.Register(registry, NoiseTypes.SHAPE, NormalNoise.Parameters.Of(-7, 1.0d, 1.0d, 2.0d, 2.0d, 2.0d, 1.0d, 1.0d));
		}
	}
}
