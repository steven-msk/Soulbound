namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.Common.Math.Random;
	using SoulboundEngine.Registry;

	public static class NoiseTypes {
		public static readonly RegistryKey<NormalNoise.Parameters> SHAPE = Create("shape");
		public static readonly RegistryKey<NormalNoise.Parameters> ROUGHNESS = Create("roughness");

		private static RegistryKey<NormalNoise.Parameters> Create(string id) {
			return RegistryKey<NormalNoise.Parameters>.Of(RegistryKeys.NOISE, Identifier.Of(id));
		}

		public static NormalNoise Instantiate(
			IRegistryEntryLookup<NormalNoise.Parameters> noises, 
			IPositionalRandomFactory random, 
			RegistryKey<NormalNoise.Parameters> noiseKey
		) {
			RegistryEntry<NormalNoise.Parameters> entry = noises.GetOrThrow(noiseKey);
			return NormalNoise.Create(random.FromHashOf(noiseKey.value), entry.GetValue());
		}
	}
}
