namespace SoulboundEngine.Client.World.Chunk {
	using Level = Level.Level;

	public readonly struct ChunkPos {
		public static readonly ChunkPos ORIGIN = new(0);

		public readonly int x;

		public ChunkPos(int x) {
			this.x = x;
		}

		public static int WorldYToIndex(int worldY) => worldY - Level.MIN_Y;

		public static int IndexToWorldY(int yIndex) => yIndex + Level.MIN_Y;

		public int WorldXToChunkX(int worldX) => worldX - this.x * Level.CHUNK_LENGTH;

		public int ChunkXToWorldX(int chunkX) => chunkX + this.x * Level.CHUNK_LENGTH;
	}
}
