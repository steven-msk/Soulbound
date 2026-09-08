namespace SoulboundEngine.World.Level {
	using SoulboundEngine.Registry;
	using SoulboundEngine.Serialization;
	using SoulboundEngine.World.Gen;

	public record LevelSettings(RegistryKey<LevelType> typeEntry, ChunkGenerator chunkGenerator) {
		public static readonly Codec<LevelSettings> CODEC = RecordCodec<LevelSettings, RegistryKey<LevelType>, ChunkGenerator>.Of(
			Field.Required<LevelSettings, RegistryKey<LevelType>>("type", LevelType.KEY_CODEC, s => s.typeEntry),
			Field.Required<LevelSettings, ChunkGenerator>("generator", ChunkGenerator.CODEC, s => s.chunkGenerator),
			(typeEntry, chunkGenerator) => new LevelSettings(typeEntry, chunkGenerator)
		);

		public static readonly RegistryKey<LevelSettings> DEFAULT = MakeKey("default");

		private static RegistryKey<LevelSettings> MakeKey(string id) {
			return RegistryKey<LevelSettings>.Of(Registries.LEVEL_SETTINGS.GetKey(), Identifier.Of(id));
		}

		public static void Init() {
		}
	}
}
