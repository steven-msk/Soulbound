namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.World.Chunk;
	using SoulboundEngine.World.Level;

	public abstract class ChunkGenerator {
		public abstract void Generate(Level level, Chunk chunk, bool placeBlocks);
	}
}
