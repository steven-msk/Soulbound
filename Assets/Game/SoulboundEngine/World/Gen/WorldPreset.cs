namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.Registry;
	using SoulboundEngine.Serialization;
	using SoulboundEngine.World.Gen.Biome;
	using SoulboundEngine.World.Level;

	public sealed record WorldPreset(LevelSettings levelSettings) {
		public static readonly Codec<WorldPreset> CODEC = RecordCodec<WorldPreset, LevelSettings>.Of(
			Field.Required<WorldPreset, LevelSettings>("levelSettings", LevelSettings.CODEC, p => p.levelSettings),
			levelSettings => new WorldPreset(levelSettings)
		);
		public static readonly RegistryKey<WorldPreset> DEFAULT = Register("default");
		public static readonly RegistryKey<WorldPreset> FLAT = Register("flat");
		public static readonly RegistryKey<WorldPreset> DEBUG_ALL_BLOCK_STATES = Register("debug_all_block_states");

		private static RegistryKey<WorldPreset> Register(string id) {
			return RegistryKey<WorldPreset>.Of(RegistryKeys.WORLD_PRESET, Identifier.Of(id));
		}

		public static WorldPreset Init(RegistryBootstrapContext context, Registry<WorldPreset> registry) {
			return new Bootstrap(context, registry).RegisterPresets();
		}

		private sealed class Bootstrap {
			private readonly RegistryBootstrapContext context;
			private readonly Registry<WorldPreset> registry;
			private readonly IRegistryEntryLookup<MultiNoiseBiomeSourceParamList> multiNoiseBiomeSourceParameterLists;
			private readonly IRegistryEntryLookup<NoiseGeneratorSettings> noiseGeneratorSettings;

			public Bootstrap(RegistryBootstrapContext context, Registry<WorldPreset> registry) {
				this.context = context;
				this.registry = registry;
				this.multiNoiseBiomeSourceParameterLists = context.Lookup(RegistryKeys.MULTI_NOISE_BIOME_SOURCE_PARAMETER_LIST);
				this.noiseGeneratorSettings = context.Lookup(RegistryKeys.NOISE_SETTINGS);
			}

			public WorldPreset RegisterPresets() {
				RegistryEntry<MultiNoiseBiomeSourceParamList> defaultPreset = this.multiNoiseBiomeSourceParameterLists.GetOrThrow(MultiNoiseBiomeSourceParamList.DEFAULT);
				BiomeSource defaultBiomeSource = MultiNoiseBiomeSource.FromPreset(defaultPreset);
				RegistryEntry<NoiseGeneratorSettings> noiseSettings = this.noiseGeneratorSettings.GetOrThrow(NoiseGeneratorSettings.DEFAULT);
				this.Register(DEFAULT, new LevelSettings(LevelType.DEFAULT, new NoiseLevelChunkGenerator(defaultBiomeSource, noiseSettings)));

				IRegistryEntryLookup<Biome.Biome> biomes = this.context.Lookup(RegistryKeys.BIOME);
				return this.Register(FLAT, new LevelSettings(LevelType.DEFAULT, new FlatLevelGenerator(FlatLevelGenerator.Settings.CreateDefault(biomes))));
			}

			private WorldPreset Register(RegistryKey<WorldPreset> key, LevelSettings settings) {
				return Registry<WorldPreset>.Register(this.registry, key, new WorldPreset(settings));
			}
		}
	}
}
