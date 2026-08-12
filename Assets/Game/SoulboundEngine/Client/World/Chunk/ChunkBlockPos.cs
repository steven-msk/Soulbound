using Newtonsoft.Json;
using SoulboundEngine.Client.World.Block;
using SoulboundEngine.Common.Json;
using UnityEngine;

namespace SoulboundEngine.Client.World.Chunk {
	using Level = Level.Level;

	[JsonConverter(typeof(ChunkBlockPosJsonConverter))]
	public struct ChunkBlockPos {
		public int x;
		public int y;
		public int chunkX;

		public ChunkBlockPos(int x, int y, int chunkX) {
			this.x = x;
			this.y = y;
			this.chunkX = chunkX;
		}

        public readonly WorldChunk UnderlyingChunk(Level level) => level.ChunkAt(this.ToBlock());

        public static ChunkBlockPos FromBlockPos(BlockPos blockPos) {
			int chunkX = Level.ChunkXAt(blockPos.x);
			int localX = Level.ToChunkX(blockPos.x);
			return new ChunkBlockPos(localX, blockPos.y, chunkX);
		}

		public static ChunkBlockPos FromWorld(Vector2 worldPos) {
			return ((BlockPos)worldPos).ToChunkPos();
		}

		public static bool operator !=(ChunkBlockPos pos1, ChunkBlockPos pos2) => !(pos1 == pos2);

		public static bool operator ==(ChunkBlockPos pos1, ChunkBlockPos pos2) {
			return pos1.x == pos2.x && pos1.y == pos2.y && pos1.chunkX == pos2.chunkX;
		}

		public static explicit operator Vector2Int(ChunkBlockPos pos) => new(pos.x, pos.y);

		public readonly override string ToString() => $"cx:{this.x}, cy:{this.y}, c:{this.chunkX}";

		public readonly BlockPos ToBlock() => new(this.x + this.chunkX * Level.CHUNK_LENGTH, this.y);

		public readonly int WorldYToIndex() => WorldYToIndex(this.y);

		public static int WorldYToIndex(int worldY) => worldY - Level.MAX_Y;


		public readonly override bool Equals(object obj) {
			if (obj is not ChunkBlockPos) {
				return false;
			}
			var other = (ChunkBlockPos)obj;
			return this == other;
		}

		public readonly override int GetHashCode() {
			unchecked {
				int hash = 17;
				hash = hash * 31 + this.x;
				hash = hash * 31 + this.y;
				hash = hash * 31 + this.chunkX;
				return hash;
			}
		}
	}
}
