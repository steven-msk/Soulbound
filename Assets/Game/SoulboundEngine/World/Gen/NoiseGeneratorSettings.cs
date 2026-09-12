namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.Registry;
	using SoulboundEngine.Serialization;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Block.State;

	public record NoiseGeneratorSettings(NoiseSize noiseSize, NoiseRouter noiseRouter, BlockState defaultBlock) {
		public static readonly Codec<RegistryEntry<NoiseGeneratorSettings>> CODEC = RegistryEntry<NoiseGeneratorSettings>.GetCodec(Registries.NOISE_SETTINGS);
		public static RegistryKey<NoiseGeneratorSettings> DEFAULT = CreateKey("default");

		private static RegistryKey<NoiseGeneratorSettings> CreateKey(string id) {
			return RegistryKey<NoiseGeneratorSettings>.Of(RegistryKeys.NOISE_SETTINGS, Identifier.Of(id));
		}

		public static NoiseGeneratorSettings Init(RegistryBootstrapContext context, Registry<NoiseGeneratorSettings> registry) {
			IRegistryEntryLookup<IDensityFunction> functions = context.Lookup(RegistryKeys.DENSITY_FUNCTION);
			IRegistryEntryLookup<NormalNoise.Parameters> noises = context.Lookup(RegistryKeys.NOISE);
			return Registry<NoiseGeneratorSettings>.Register(registry, DEFAULT, new NoiseGeneratorSettings(
				NoiseSize.DEFAULT, NoiseRouter.CreateDefault(functions, noises), Blocks.STONE.DefaultState
			));
		}
	}
}
