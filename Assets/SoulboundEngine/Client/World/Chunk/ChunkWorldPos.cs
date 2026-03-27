using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Core;
using UnityEngine;

namespace SoulboundEngine.Client.World.Chunk {
	public struct ChunkWorldPos {
		public float x;
		public float y;
		public int chunkX;

		public ChunkWorldPos(float x, float y, int chunkX) {
			this.x = x;
			this.y = y;
			this.chunkX = chunkX;
		}

        public static ChunkWorldPos FromBlockPos(BlockPos blockPos) {
			int chunkX = Mathf.FloorToInt((float)blockPos.x / Level.CHUNK_LENGTH);
			float localX = blockPos.x - chunkX * Level.CHUNK_LENGTH;
			return new ChunkWorldPos(localX, blockPos.y, chunkX);
		}

		public static ChunkWorldPos FromWorld(Vector2 position) {
			int chunkX = Mathf.FloorToInt(position.x / Level.CHUNK_LENGTH);
			return new ChunkWorldPos(position.x, position.y, chunkX);
		}

		public static bool operator !=(ChunkWorldPos pos1, ChunkWorldPos pos2) => !(pos1 == pos2);

		public static bool operator ==(ChunkWorldPos pos1, ChunkWorldPos pos2) {
			return pos1.x == pos2.x && pos1.y == pos2.y && pos1.chunkX == pos2.chunkX;
		}

		public override readonly string ToString() => $"cwx:{x}, cwy:{y}, c:{chunkX}";

		public readonly BlockPos ToWorldBlockPos() {
			Vector2 pos = new(x + chunkX * Level.CHUNK_LENGTH, y);
			return (BlockPos)pos;
		}

		public override readonly bool Equals(object obj) {
			if (obj is not ChunkWorldPos) {
				return false;
			}
			var other = (ChunkWorldPos)obj;
			return this == other;
		}

		public override readonly int GetHashCode() {
			unchecked {
				int hash = 17;
				hash = hash * 31 + (int)x;
				hash = hash * 31 + (int)y;
				hash = hash * 31 + chunkX;
				return hash;
			}
		}
	}
}
