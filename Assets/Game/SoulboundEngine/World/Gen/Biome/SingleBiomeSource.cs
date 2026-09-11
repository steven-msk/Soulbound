namespace SoulboundEngine.World.Gen.Biome {
	using SoulboundEngine.Registry;
	using System.Collections.Generic;

	public sealed class SingleBiomeSource : BiomeSource {
		private readonly RegistryEntry<Biome> biome;

		public SingleBiomeSource(RegistryEntry<Biome> biome) {
			this.biome = biome;
		}

		public override RegistryEntry<Biome> GetNoiseBiome(int x, int y, Climate.Sampler sampler) {
			return this.biome;
		}

		protected override IEnumerable<RegistryEntry<Biome>> CollectPossibleBiomes() {
			yield return this.biome;
		}
	}
}
