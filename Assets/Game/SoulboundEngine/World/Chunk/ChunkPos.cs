namespace SoulboundEngine.World.Chunk {
	using SoulboundEngine.World.Level;
	using System;

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

		public static ChunkPos Parse(string s) {
			if (!s.StartsWith("chunk[")) throw new ArgumentException("Cannot parse chunk pos: " + s);

			int start = "chunk[".Length;
			int end = s.IndexOf(']', start);
			if (end < 0) throw new ArgumentException("Cannot parse chunk pos: " + s);

			string num = s[start..end];
			if (string.IsNullOrEmpty(num)) throw new ArgumentException("Cannot parse chunk pos: " + s);

			for (int j = 0; j < num.Length; j++) {
				char c = num[j];
				bool validDigit = char.IsDigit(c);
				bool validSign = c == '-' && j == 0;
				if (!validDigit && !validSign) throw new ArgumentException("Cannot parse chunk pos: " + s);
			}

			return new ChunkPos(int.Parse(num));
		}

		public override string ToString() => $"chunk[{this.x}]";
	}
}
