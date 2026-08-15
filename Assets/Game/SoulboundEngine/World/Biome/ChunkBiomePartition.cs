#nullable enable

namespace SoulboundEngine.World.Biome {
	public struct ChunkBiomePartition {
		public IBiome primary;
		public IBiome? secondary;
		public int splitX;

		public readonly bool hasBorder => this.secondary != null;
	}
}
