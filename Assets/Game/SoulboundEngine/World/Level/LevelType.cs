namespace SoulboundEngine.World.Level {
	using SoulboundEngine.Registry;
	using SoulboundEngine.Serialization;

	public record LevelType(int minY, int maxY) {
		public static readonly Codec<RegistryKey<LevelType>> KEY_CODEC = RegistryKey<LevelType>.Codec(RegistryKeys.LEVEL_TYPE);
		public static readonly RegistryKey<LevelType> DEFAULT = Register("default");

		public int Height => this.maxY - this.minY;

		private static RegistryKey<LevelType> Register(string id) {
			return RegistryKey<LevelType>.Of(RegistryKeys.LEVEL_TYPE, Identifier.Of(id));
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter")]
		public static LevelType Init(RegistryBootstrapContext context, Registry<LevelType> registry) {
			return Registry<LevelType>.Register(registry, DEFAULT, new LevelType(Level.DEFAULT_MIN_Y, Level.DEFAULT_MAX_Y));
		}
	}
}
