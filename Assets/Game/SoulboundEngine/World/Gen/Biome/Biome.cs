namespace SoulboundEngine.World.Gen.Biome {
	using SoulboundEngine.Registry;
	using SoulboundEngine.Serialization;

	public sealed class Biome {
		public static readonly Codec<RegistryEntry<Biome>> ENTRY_CODEC = RegistryEntry<Biome>.GetCodec(Registries.BIOME);
		public static readonly RegistryKey<Biome> PLAINS = Register("plains");
		public static readonly RegistryKey<Biome> HILLS = Register("hills");
		public BiomeGenSettings generationSettings { get; }

		private static RegistryKey<Biome> Register(string id) {
			return RegistryKey<Biome>.Of(RegistryKeys.BIOME, Identifier.Of(id));
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter")]
		public static Biome Init(RegistryBootstrapContext context, Registry<Biome> registry) {
			Registry<Biome>.Register(registry, PLAINS, new Biome(BiomeGenSettings.EMPTY));
			return Registry<Biome>.Register(registry, HILLS, new Biome(BiomeGenSettings.EMPTY));
		}

		private Biome(BiomeGenSettings generationSettings) {
			this.generationSettings = generationSettings;
		}
	}
}
