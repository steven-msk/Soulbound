using SoulboundBackend.Common.Json;
using SoulboundBackend.Core;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace SoulboundBackend.Client.World.Chunk {
	[JsonConverter(typeof(ChunkBlockPosJsonConverter))]
	public struct ChunkBlockPos {
		public int x;
		public int y;
		public int chunkX;
		[JsonIgnore] public WorldChunk underlyingChunk => LevelManager.instance.Level.ChunkAt(this.ToWorldBlockPos());

		public ChunkBlockPos(int x, int y, int chunkX) {
			this.x = x;
			this.y = y;
			this.chunkX = chunkX;
		}

		public static ChunkBlockPos FromBlockPos(BlockPos blockPos) {
			int chunkX = Mathf.FloorToInt((float)blockPos.x / Level.CHUNK_LENGTH);
			int localX = blockPos.x - chunkX * Level.CHUNK_LENGTH;
			return new ChunkBlockPos(localX, blockPos.y, chunkX);
		}

		public static ChunkBlockPos FromWorld(Vector2 position) => FromBlockPos(BlockPos.FromWorld(position));

		public static bool operator !=(ChunkBlockPos pos1, ChunkBlockPos pos2) => !(pos1 == pos2);

		public static bool operator ==(ChunkBlockPos pos1, ChunkBlockPos pos2) {
			return pos1.x == pos2.x && pos1.y == pos2.y && pos1.chunkX == pos2.chunkX;
		}

		public static explicit operator Vector2Int(ChunkBlockPos pos) => new Vector2Int(pos.x, pos.y);

		public override string ToString() => $"cx:{x}, cy:{y}, c:{chunkX}";

		public BlockPos ToWorldBlockPos() => new BlockPos(this.x + this.chunkX * Level.CHUNK_LENGTH, this.y);

		public int WorldYToIndex() => WorldYToIndex(this.y);

		public static int WorldYToIndex(int worldY) => worldY - WorldChunk.maxY;


		public override bool Equals(object obj) {
			if (!(obj is ChunkBlockPos)) {
				return false;
			}
			var other = (ChunkBlockPos)obj;
			return this == other;
		}

		public override int GetHashCode() {
			unchecked {
				int hash = 17;
				hash = hash * 31 + x;
				hash = hash * 31 + y;
				hash = hash * 31 + chunkX;
				return hash;
			}
		}
	}
}
