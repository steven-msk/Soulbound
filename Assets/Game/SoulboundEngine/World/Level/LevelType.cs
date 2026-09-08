namespace SoulboundEngine.World.Level {
	using SoulboundEngine.Registry;
	using SoulboundEngine.Serialization;

	public record LevelType(int minY, int maxY) {
		public static readonly Codec<RegistryKey<LevelType>> KEY_CODEC = RegistryKey<LevelType>.Codec(Registries.LEVEL_TYPE.GetKey());
		public static readonly RegistryKey<LevelType> DEFAULT = Register("default");

		private static RegistryKey<LevelType> Register(string id) {
			return RegistryKey<LevelType>.Of(Registries.LEVEL_TYPE.GetKey(), Identifier.Of(id));
		}

		public static void Init() {
		}
	}
}
