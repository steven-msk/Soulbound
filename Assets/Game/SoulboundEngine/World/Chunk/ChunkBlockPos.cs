namespace SoulboundEngine.World.Chunk {
	using SoulboundEngine.Common.Math;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Level;

	public struct ChunkBlockPos {
		public int xInChunk;
		public int chunkY;
		public ChunkPos chunkPos;

		public ChunkBlockPos(int chunkX, int chunkY, ChunkPos chunkPos) {
			this.xInChunk = chunkX;
			this.chunkY = chunkY;
			this.chunkPos = chunkPos;
		}

        public readonly Chunk UnderlyingChunk(Level level) => level.GetChunk(this.ToBlock());

        public static ChunkBlockPos FromBlockPos(BlockPos blockPos) {
			int chunkPos = SectionPos.BlockToSectionCoord(blockPos.x);
			int localX = blockPos.x - chunkPos * ChunkSection.WIDTH;
			return new ChunkBlockPos(localX, blockPos.y, new ChunkPos(chunkPos));
		}

		public static ChunkBlockPos FromWorld(Vec2d worldPos) {
			return BlockPos.From(worldPos).ToChunkPos();
		}

		public static bool operator !=(ChunkBlockPos pos1, ChunkBlockPos pos2) => !(pos1 == pos2);

		public static bool operator ==(ChunkBlockPos pos1, ChunkBlockPos pos2) {
			return pos1.xInChunk == pos2.xInChunk && pos1.chunkY == pos2.chunkY && pos1.chunkPos == pos2.chunkPos;
		}

		public readonly override string ToString() => $"cx:{this.xInChunk}, cy:{this.chunkY}, c:{this.chunkPos.x}";

		public readonly BlockPos ToBlock() => new(this.chunkPos.ToWorldX(this.xInChunk), this.chunkY);

		public static int ToLocalX(int blockX) => blockX - SectionPos.BlockToSectionCoord(blockX) * ChunkSection.WIDTH;

		public readonly override bool Equals(object obj) {
			if (obj is not ChunkBlockPos) {
				return false;
			}
			ChunkBlockPos other = (ChunkBlockPos)obj;
			return this == other;
		}

		public readonly override int GetHashCode() {
			unchecked {
				int hash = 17;
				hash = hash * 31 + this.xInChunk;
				hash = hash * 31 + this.chunkY;
				hash = hash * 31 + this.chunkPos.x;
				return hash;
			}
		}
	}
}
