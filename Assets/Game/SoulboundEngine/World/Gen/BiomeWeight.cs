using SoulboundEngine.World.Biome;

namespace SoulboundEngine.World.Gen {
	public struct BiomeWeight {
		public IBiome biome;
		public float value;

		public BiomeWeight(IBiome biome, float value) {
			this.biome = biome;
			this.value = value;
		}
	}
}
