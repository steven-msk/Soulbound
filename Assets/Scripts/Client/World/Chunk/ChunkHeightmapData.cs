using System.Collections.Generic;

namespace SoulboundBackend.Client.World.Chunk {
	public struct ChunkHeightmapData {
		/// <summary>
		/// worldX -> surface level mapping
		/// </summary>
		public Dictionary<int, int> surfaceLevels;
		public int highestUndergroundLevel;

		public ChunkHeightmapData(Dictionary<int, int> surfaceLevels, int highestUndergroundLevel) {
			this.surfaceLevels = surfaceLevels;
			this.highestUndergroundLevel = highestUndergroundLevel;
		}
	}
}
