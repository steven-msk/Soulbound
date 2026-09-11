namespace SoulboundEngine.World.Gen.Biome {
	using SoulboundEngine.Registry;

	public sealed class Biome {
		public static readonly RegistryKey<Biome> PLAINS = Register("plains");
		public static readonly RegistryKey<Biome> HILLS = Register("hills");

		private static RegistryKey<Biome> Register(string id) {
			return RegistryKey<Biome>.Of(RegistryKeys.BIOME, Identifier.Of(id));
		}

		public static Biome Init(Registry<Biome> registry) {
			Registry<Biome>.Register(registry, PLAINS, new Biome());
			return Registry<Biome>.Register(registry, HILLS, new Biome());
		}
	}
}
