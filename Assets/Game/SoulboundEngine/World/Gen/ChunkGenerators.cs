namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.Registry;
	using SoulboundEngine.Serialization;

	public static class ChunkGenerators {
		public static readonly RegistryKey<MapCodec<ChunkGenerator>> NOISE = Register("noise", NoiseLevelChunkGenerator.CODEC);

		private static RegistryKey<MapCodec<ChunkGenerator>> Register(string id, MapCodec<ChunkGenerator> codec) {
			RegistryKey<MapCodec<ChunkGenerator>> key = KeyOf(id);
			Registry<MapCodec<ChunkGenerator>>.Register(Registries.CHUNK_GENERATOR, key, codec);
			return key;
		}

		private static RegistryKey<MapCodec<ChunkGenerator>> KeyOf(string id) {
			return RegistryKey<MapCodec<ChunkGenerator>>.Of(Registries.CHUNK_GENERATOR.GetKey(), Identifier.Of(id));
		}

		public static void Init() {
		}
	}
}
