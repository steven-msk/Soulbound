namespace SoulboundEngine.World.Gen.Biome {
	using SoulboundEngine.Registry;
	using SoulboundEngine.Serialization;
	using System.Collections.Generic;

	public sealed class SingleBiomeSource : BiomeSource {
		public new static readonly MapCodec<BiomeSource> CODEC = RecordMapCodec<BiomeSource, RegistryEntry<Biome>>.Of(
			Field.Required<BiomeSource, RegistryEntry<Biome>>("biome", Biome.ENTRY_CODEC, s => ((SingleBiomeSource)s).biome),
			biome => new SingleBiomeSource(biome)
		);

		private readonly RegistryEntry<Biome> biome;

		public SingleBiomeSource(RegistryEntry<Biome> biome) {
			this.biome = biome;
		}

		public override RegistryEntry<Biome> GetNoiseBiome(int x, int y, Climate.Sampler sampler) {
			return this.biome;
		}

		protected override MapCodec<BiomeSource> Codec() => CODEC;

		protected override IEnumerable<RegistryEntry<Biome>> CollectPossibleBiomes() {
			yield return this.biome;
		}
	}
}
