namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.Registry;
	using SoulboundEngine.Serialization;
	using SoulboundEngine.World.Chunk;
	using SoulboundEngine.World.Level;

	public abstract class ChunkGenerator {
		public static readonly Codec<ChunkGenerator> CODEC = RegistryEntry<Codec<ChunkGenerator>>.MapCodec(Registries.CHUNK_GENERATOR).Xmap<ChunkGenerator>(???, g => g.Codec());

		public abstract void Generate(Level level, Chunk chunk, bool placeBlocks);

		protected abstract Codec<ChunkGenerator> Codec();
	}
}
