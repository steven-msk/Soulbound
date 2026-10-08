namespace SoulboundEngine.World.Level {
	using SoulboundEngine.World.Chunk;

	public interface ILevelAccess : IRegistryLevelView {
		ChunkManager GetChunkManager();

		public virtual bool HasChunk(int chunkX) {
			return this.GetChunkManager().HasChunk(chunkX);
		}
	}

	public static class LevelAccessDefaults {
		public static bool HasChunk(this ILevelAccess levelAccess, int chunkX) {
			return levelAccess.HasChunk(chunkX);
		}
	}
}
