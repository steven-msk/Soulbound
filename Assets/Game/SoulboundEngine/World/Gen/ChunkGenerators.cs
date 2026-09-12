namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.Registry;
	using SoulboundEngine.Serialization;

	public static class ChunkGenerators {
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter")]
		public static MapCodec<ChunkGenerator> Init(RegistryBootstrapContext context, Registry<MapCodec<ChunkGenerator>> registry) {
			Registry<MapCodec<ChunkGenerator>>.Register(registry, "flat", FlatLevelGenerator.CODEC);
			return Registry<MapCodec<ChunkGenerator>>.Register(registry, "noise", NoiseLevelChunkGenerator.CODEC);
		}
	}
}
