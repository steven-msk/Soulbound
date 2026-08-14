namespace SoulboundEngine.Client.World.Gen {
	using Chunk = Chunk.Chunk;
	using Level = Level.Level;

	public abstract class ChunkGenerator {
		public abstract void Generate(Level level, Chunk chunk, bool placeBlocks);
	}
}
