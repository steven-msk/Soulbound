namespace SoulboundEngine.World.Chunk {
	using SoulboundEngine.Serialization;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Level;
	using System;

	public readonly struct ChunkPos : IEquatable<ChunkPos> {
		public static readonly ChunkPos ORIGIN = new(0);
		public static readonly Codec<ChunkPos> CODEC = Codecs.INT.Xmap(x => new ChunkPos(x), c => c.x);
		public readonly int x;

		public ChunkPos(int x) {
			this.x = x;
		}

		public static int WorldYToIndex(int worldY) => worldY - Level.DEFAULT_MIN_Y;

		public static int IndexToWorldY(int yIndex) => yIndex + Level.DEFAULT_MIN_Y;

		public int ToLocalX(int worldX) => worldX - this.x * Level.CHUNK_LENGTH;

		public int ToWorldX(int chunkX) => chunkX + this.x * Level.CHUNK_LENGTH;

		public static ChunkPos Containing(BlockPos pos) {
			return new ChunkPos(SectionPos.BlockToSectionCoord(pos.x));
		}

		public override string ToString() => $"chunk[{this.x}]";

		public static bool operator ==(ChunkPos a, ChunkPos b) => a.Equals(b);

		public static bool operator !=(ChunkPos a, ChunkPos b) => !(a == b);

		public override bool Equals(object obj) {
			return obj is ChunkPos other && other.Equals(this);
		}

		public bool Equals(ChunkPos other) => other.x == this.x;

		public override int GetHashCode() {
			return HashCode.Combine(this.x);
		}
	}
}
