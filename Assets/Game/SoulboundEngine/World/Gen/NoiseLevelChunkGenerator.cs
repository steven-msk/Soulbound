namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.Registry;
	using SoulboundEngine.Serialization;
	using SoulboundEngine.World.Chunk;
	using SoulboundEngine.World.Gen.Biome;
	using System;

#nullable enable

	public sealed class NoiseLevelChunkGenerator : ChunkGenerator {
		public new static readonly MapCodec<ChunkGenerator> CODEC = RecordMapCodec<ChunkGenerator, BiomeSource, RegistryEntry<NoiseGeneratorSettings>>.Of(
			Field.Required<ChunkGenerator, BiomeSource>("biome_source", BiomeSource.CODEC, g => ((NoiseLevelChunkGenerator)g).biomeSource),
			Field.Required<ChunkGenerator, RegistryEntry<NoiseGeneratorSettings>>("noise_settings", NoiseGeneratorSettings.CODEC, g => ((NoiseLevelChunkGenerator)g).noiseSettings),
			(biomeSource, noiseSettings) => new NoiseLevelChunkGenerator(biomeSource, noiseSettings)
		);
		private readonly RegistryEntry<NoiseGeneratorSettings> noiseSettings;

		public NoiseLevelChunkGenerator(BiomeSource biomeSource, RegistryEntry<NoiseGeneratorSettings> noiseSettings)
			: base(biomeSource) {
			this.noiseSettings = noiseSettings;
		}

		protected override MapCodec<ChunkGenerator> Codec() => CODEC;

		public override Chunk Fill(RandomState randomState, Chunk chunk) {
			throw new NotImplementedException();
		}

		public override Chunk BuildSurface(RandomState randomState, Chunk chunk) {
			throw new NotImplementedException();
		}

		public override int GetMinGenY() {
			throw new NotImplementedException();
		}

		public override int GetGenHeight() {
			throw new NotImplementedException();
		}

		public override int GetBaseHeight(int x, IHeightLimitView heightLimit) {
			throw new NotImplementedException();
		}
	}
}
