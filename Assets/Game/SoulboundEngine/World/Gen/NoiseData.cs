namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.Registry;

	public static class NoiseData {
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter")]
		public static NormalNoise.Parameters Init(RegistryBootstrapContext context, Registry<NormalNoise.Parameters> registry) {
			return Registry<NormalNoise.Parameters>.Register(registry, NoiseTypes.SHAPE, NormalNoise.Parameters.Of(-7, 1.0d, 1.0d, 2.0d, 2.0d, 2.0d, 1.0d, 1.0d));
		}
	}
}
