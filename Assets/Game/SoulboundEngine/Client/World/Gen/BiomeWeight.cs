using SoulboundEngine.Client.World.Biome;

namespace SoulboundEngine.Client.World.Gen {
	public struct BiomeWeight {
		public IBiome biome;
		public float value;

		public BiomeWeight(IBiome biome, float value) {
			this.biome = biome;
			this.value = value;
		}
	}
}
