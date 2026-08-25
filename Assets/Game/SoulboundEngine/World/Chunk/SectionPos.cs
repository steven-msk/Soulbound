namespace SoulboundEngine.World.Chunk {
	using SoulboundEngine.Common.Math;

	public readonly struct SectionPos {
		public readonly int x;
		public readonly int y;

		public SectionPos(int x, int y) {
			this.x = x;
			this.y = y;
		}

		public override string ToString() {
			return $"sectionPos[{this.x},{this.y}]";
		}

		public static int BlockToSectionY(int blockY) => Maths.FloorToInt(1.0d * blockY / ChunkSection.HEIGHT);

		public static int BlockToSectionCoord(int coord) => Maths.FloorToInt(1.0d * coord / ChunkSection.WIDTH);
	}
}
