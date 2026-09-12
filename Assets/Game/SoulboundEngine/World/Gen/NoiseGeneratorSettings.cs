namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.Common.Math.Random;
	using SoulboundEngine.Registry;
	using SoulboundEngine.Serialization;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Block.State;
	using System;

	public record NoiseGeneratorSettings(
			Func<long, IRandom> randomFactory,
			NoiseSize noiseSize, 
			NoiseRouter noiseRouter, 
			BlockState defaultBlock,
			int baseHeight
		) {
		public static readonly Codec<RegistryEntry<NoiseGeneratorSettings>> CODEC = RegistryEntry<NoiseGeneratorSettings>.GetCodec(Registries.NOISE_SETTINGS);
		public static RegistryKey<NoiseGeneratorSettings> DEFAULT = CreateKey("default");
		public static RegistryKey<NoiseGeneratorSettings> ZERO = CreateKey("zero");

		private static RegistryKey<NoiseGeneratorSettings> CreateKey(string id) {
			return RegistryKey<NoiseGeneratorSettings>.Of(RegistryKeys.NOISE_SETTINGS, Identifier.Of(id));
		}

		public static NoiseGeneratorSettings Zero() {
			return new NoiseGeneratorSettings(
				seed => new Xoshiro256StarStarRandom(seed),
				NoiseSize.DEFAULT, 
				NoiseRouter.Zero(),
				Blocks.STONE.DefaultState,
				baseHeight: 0
			);
		}

		public static NoiseGeneratorSettings GetDefault(IRegistryEntryLookup<IDensityFunction> functions, IRegistryEntryLookup<NormalNoise.Parameters> noises) {
			return new NoiseGeneratorSettings(
				seed => new Xoshiro256StarStarRandom(seed),
				NoiseSize.DEFAULT,
				NoiseRouter.CreateDefault(functions, noises),
				Blocks.STONE.DefaultState,
				baseHeight: 0
			);
		}

		public static NoiseGeneratorSettings Init(RegistryBootstrapContext context, Registry<NoiseGeneratorSettings> registry) {
			IRegistryEntryLookup<IDensityFunction> functions = context.Lookup(RegistryKeys.DENSITY_FUNCTION);
			IRegistryEntryLookup<NormalNoise.Parameters> noises = context.Lookup(RegistryKeys.NOISE);

			Registry<NoiseGeneratorSettings>.Register(registry, DEFAULT, GetDefault(functions, noises));
			return Registry<NoiseGeneratorSettings>.Register(registry, ZERO, Zero());
		}
	}
}
