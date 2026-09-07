namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.Registry;
	using SoulboundEngine.Serialization;
	using SoulboundEngine.World.Level;

	public readonly struct WorldPreset {
		public static readonly Codec<WorldPreset> CODEC = RecordCodec<WorldPreset, LevelSettings>.Of(
			Field.Required<WorldPreset, LevelSettings>("levelSettings", LevelSettings.CODEC, p => p.levelSettings),
			levelSettings => new WorldPreset(levelSettings)
		);
		public static readonly RegistryKey<WorldPreset> DEFAULT = Register("default");
		public static readonly RegistryKey<WorldPreset> FLAT = Register("flat");
		public static readonly RegistryKey<WorldPreset> DEBUG_ALL_BLOCK_STATES = Register("debug_all_block_states");
		public readonly LevelSettings levelSettings;

		public WorldPreset(LevelSettings levelSettings) {
			this.levelSettings = levelSettings;
		}

		private static RegistryKey<WorldPreset> Register(string id) {
			return RegistryKey<WorldPreset>.Of(Registries.WORLD_PRESET.GetKey(), Identifier.Of(id));
		}
	}
}
