namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.Registry;
	using SoulboundEngine.Serialization;
	using SoulboundEngine.World.Biome;
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

		public static WorldPreset Init(Registry<WorldPreset> registry) {
			int seed = 0;
			PlainsBiome biome1 = new(seed);
			HillsBiome biome2 = new(seed);
			BiomeMap biomeMap = new(new IBiome[] { biome1, biome2 });
			Heightmap heightmap = new(0);
			Cavemap cavemap = new(seed);
			return Registry<WorldPreset>.Register(registry, DEFAULT, new WorldPreset(new LevelSettings(LevelType.DEFAULT, new NoiseLevelChunkGenerator(biomeMap, heightmap, cavemap))));
		}
	}
}
