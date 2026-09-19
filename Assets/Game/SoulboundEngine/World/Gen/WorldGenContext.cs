namespace SoulboundEngine.World.Gen {
	using System;

	public class WorldGenContext {
		public int minGenY { get; }
		public int height { get; }

		public WorldGenContext(ChunkGenerator chunkGenerator, IHeightLimitView heightLimit) {
			this.minGenY = Math.Max(chunkGenerator.GetMinGenY(), heightLimit.GetBottomY());
			this.height = Math.Min(chunkGenerator.GetGenHeight(), heightLimit.GetHeight());
		}

	}
}
