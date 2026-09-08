namespace SoulboundEngine.World.Level {
	using SoulboundEngine.Registry;
	using SoulboundEngine.Serialization;

	public record LevelType(int minY, int maxY) {
		public static readonly Codec<RegistryKey<LevelType>> KEY_CODEC = RegistryKey<LevelType>.Codec(Registries.LEVEL_TYPE.GetKey());
		public static readonly RegistryKey<LevelType> DEFAULT = Register("default");

		private static RegistryKey<LevelType> Register(string id) {
			return RegistryKey<LevelType>.Of(RegistryKeys.LEVEL_TYPE, Identifier.Of(id));
		}

		public static LevelType Init(Registry<LevelType> registry) {
			return Registry<LevelType>.Register(registry, DEFAULT, new LevelType(Level.MIN_Y, Level.MAX_Y));
		}
	}
}
