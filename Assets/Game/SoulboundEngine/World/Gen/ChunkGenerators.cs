namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.Registry;
	using SoulboundEngine.Serialization;

	public static class ChunkGenerators {
		public static MapCodec<ChunkGenerator> Init(Registry<MapCodec<ChunkGenerator>> registry) {
			return Registry<MapCodec<ChunkGenerator>>.Register(registry, "noise", NoiseLevelChunkGenerator.CODEC);
		}
	}
}
