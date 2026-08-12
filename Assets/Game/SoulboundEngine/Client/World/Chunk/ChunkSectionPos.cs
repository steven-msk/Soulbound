using SoulboundEngine.Client.World.Block;

namespace SoulboundEngine.Client.World.Chunk {
	public readonly struct ChunkSectionPos {
		public readonly int x;
		public readonly int y;
		private readonly int sectionX;
		private readonly int sectionY;

		public ChunkSectionPos(int x, int y, int sectionX, int sectionY) {
			this.x = x;
			this.y = y;
			this.sectionX = sectionX;
			this.sectionY = sectionY;
		}

		public BlockPos ToBlockPos() {
			return new BlockPos(this.x + this.sectionX * ChunkSection.WIDTH, this.y + this.sectionY * ChunkSection.HEIGHT);
		}

		public static int BlockToSectionY(int blockY) => blockY / ChunkSection.HEIGHT;
	}
}
