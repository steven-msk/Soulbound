namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.Registry;
	using SoulboundEngine.Serialization;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Block.State;
	using SoulboundEngine.World.Chunk;
	using SoulboundEngine.World.Gen.Biome;
	using SoulboundEngine.World.Gen.Function;
	using SoulboundEngine.World.Gen.Noise;
	using SoulboundEngine.World.Level;
	using System;

#nullable enable

	public sealed class NoiseLevelChunkGenerator : ChunkGenerator {
		public new static readonly MapCodec<ChunkGenerator> CODEC = RecordMapCodec<ChunkGenerator, BiomeSource, RegistryEntry<NoiseGeneratorSettings>>.Of(
			Field.Required<ChunkGenerator, BiomeSource>("biome_source", BiomeSource.CODEC, g => ((NoiseLevelChunkGenerator)g).biomeSource),
			Field.Required<ChunkGenerator, RegistryEntry<NoiseGeneratorSettings>>("noise_settings", NoiseGeneratorSettings.CODEC, g => ((NoiseLevelChunkGenerator)g).noiseSettings),
			(biomeSource, noiseSettings) => new NoiseLevelChunkGenerator(biomeSource, noiseSettings)
		);
		private static readonly BlockState AIR = Blocks.AIR.DefaultState;
		private readonly RegistryEntry<NoiseGeneratorSettings> noiseSettings;

		public NoiseLevelChunkGenerator(BiomeSource biomeSource, RegistryEntry<NoiseGeneratorSettings> noiseSettings)
			: base(biomeSource) {
			this.noiseSettings = noiseSettings;
		}

		protected override MapCodec<ChunkGenerator> Codec() => CODEC;

		public RegistryEntry<NoiseGeneratorSettings> NoiseSettings => this.noiseSettings;

		public override Chunk Fill(RandomState randomState, Chunk chunk) {
			IDensityFunction finalDensity = randomState.Router.finalTerrain;
			Heightmap heightmap = chunk.GetHeightmap();
			BlockPos.Mutable blockPos = new();

			for (int cx = 0; cx < Level.CHUNK_LENGTH; cx++) {
				int worldX = chunk.GetPos().ToWorldX(cx);
				int topmostSolid = int.MinValue;

				for (int y = chunk.GetTopY(); y >= this.GetMinGenY(); y--) {
					blockPos.Set(worldX, y);
					double density = finalDensity.Compute(new IDensityFunction.SinglePointContext(worldX, y));
					if (density > 0) {
						chunk.SetBlockState(blockPos, this.noiseSettings.GetValue().defaultBlock);
						if (topmostSolid == int.MinValue) topmostSolid = y;
					}
				}

				heightmap.Update(cx, topmostSolid, chunk.GetBlockState(blockPos.Set(worldX, topmostSolid)));
			}
			return chunk;
		}

		public override Chunk BuildSurface(RandomState randomState, Chunk chunk) {
			randomState.SurfaceBuilder.BuildSurface(randomState, chunk, this.noiseSettings.GetValue().surfaceRule);
			return chunk;
		}

		public override int GetMinGenY() => Level.DEFAULT_MIN_Y;

		public override int GetGenHeight() => Level.DEFAULT_WORLD_HEIGHT;

		public override int GetBaseHeight(RandomState randomState, int x, IHeightLimitView heightLimit) {
			return (int)Math.Round(randomState.Router.terrainHeight.Compute(new IDensityFunction.SinglePointContext(x, 0)));
		}

	}
}
