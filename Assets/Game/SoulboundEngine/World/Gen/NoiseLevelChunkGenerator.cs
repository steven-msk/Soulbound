namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.Registry;
	using SoulboundEngine.Serialization;
	using SoulboundEngine.World.Chunk;
	using SoulboundEngine.World.Gen.Biome;
	using System;

#nullable enable

	public sealed class NoiseLevelChunkGenerator : ChunkGenerator {
		// TEMPORARY
		public new static readonly MapCodec<ChunkGenerator> CODEC = RecordMapCodec<ChunkGenerator, int>.Of(
			Field.Required<ChunkGenerator, int>("temp", Codecs.INT, v => 1),
			i => {
				Logger.LogError("temporary codec hit!");
				return default;
			}
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
