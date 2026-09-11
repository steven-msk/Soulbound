namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.Registry;

	public static class NoiseTypes {
		public static readonly RegistryKey<NormalNoise.Parameters> SHAPE = Create("shape");

		private static RegistryKey<NormalNoise.Parameters> Create(string id) {
			return RegistryKey<NormalNoise.Parameters>.Of(RegistryKeys.NOISE, Identifier.Of(id));
		}
	}
}
