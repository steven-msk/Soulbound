namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.Registry;
	using SoulboundEngine.Serialization;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Chunk;
	using SoulboundEngine.World.Gen.Biome;
	using SoulboundEngine.World.Level;
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

		public RegistryEntry<NoiseGeneratorSettings> NoiseSettings => this.noiseSettings;

		public override Chunk Fill(RandomState randomState, Chunk chunk) {
			NoiseGeneratorSettings settings = this.noiseSettings.GetValue();
			IDensityFunction shapeFunction = randomState.Router.GetParameterNoise(Climate.ParameterType.SHAPE);
			Heightmap heightmap = chunk.GetHeightmap();
			BlockPos.Mutable blockPos = new();

			for (int x = 0; x < Level.CHUNK_LENGTH; x++) {
				int blockX = chunk.GetPos().ToWorldX(x);
				int height = this.SampleHeight(shapeFunction, blockX, settings.baseHeight, 1f);

				for (int y = this.GetMinGenY(); y < height; y++) {
					chunk.SetBlockState(blockPos.Set(blockX, y), settings.defaultBlock);
				}
				heightmap.Update(x, height, settings.defaultBlock);
			}
			return chunk;
		}

		public override Chunk BuildSurface(RandomState randomState, Chunk chunk) {
			return chunk;
		}

		public override int GetMinGenY() => Level.DEFAULT_MIN_Y;

		public override int GetGenHeight() => Level.DEFAULT_WORLD_HEIGHT;

		public override int GetBaseHeight(RandomState randomState, int x, IHeightLimitView heightLimit) {
			NoiseGeneratorSettings settings = this.noiseSettings.GetValue();
			IDensityFunction shapeFunction = randomState.Router.GetParameterNoise(Climate.ParameterType.SHAPE);
			return this.SampleHeight(shapeFunction, x, settings.baseHeight, 1f);
		}

		private int SampleHeight(IDensityFunction densityFunction, int blockX, int baseHeight, float amplitude) {
			double density = densityFunction.Compute(new IDensityFunction.SinglePointContext(blockX, 0));
			return baseHeight + (int)Math.Round(density * amplitude);
		}
	}
}
