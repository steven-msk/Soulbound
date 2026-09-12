namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.Common;
	using SoulboundEngine.Registry;
	using SoulboundEngine.Serialization;
	using SoulboundEngine.World.Chunk;
	using SoulboundEngine.World.Gen.Biome;

	public abstract class ChunkGenerator {
		public static readonly Codec<ChunkGenerator> CODEC = new DispatchCodec<ChunkGenerator, RegistryKey<MapCodec<ChunkGenerator>>>(
			RegistryKey<MapCodec<ChunkGenerator>>.Codec(RegistryKeys.CHUNK_GENERATOR),
			"key",
			g => Registries.CHUNK_GENERATOR.GetKey(g.Codec()),
			key => {
				Optional<MapCodec<ChunkGenerator>> entry = Optional<MapCodec<ChunkGenerator>>.Of(Registries.CHUNK_GENERATOR.Get(key)?.GetValue());
				return entry.IsPresent()
					? DataResult<MapCodec<ChunkGenerator>>.Success(entry.GetValue())
					: DataResult<MapCodec<ChunkGenerator>>.Error($"Unknown chunk generator {key}");
			}
		);
		protected readonly BiomeSource biomeSource;

		public ChunkGenerator(BiomeSource biomeSource) {
			this.biomeSource = biomeSource;
		}

		/// <summary>
		/// <b>Must return a stable instance.
		/// Reverse lookup will fail if the codec is constructed fresh every call</b>
		/// </summary>
		protected abstract MapCodec<ChunkGenerator> Codec();

		public virtual Chunk MapBiomes(RandomState randomState, Chunk chunk) {
			chunk.FillBiomesFromNoise(this.biomeSource, randomState.Sampler);
			return chunk;
		}

		public abstract Chunk Fill(RandomState randomState, Chunk chunk);

		public abstract Chunk BuildSurface(RandomState randomState, Chunk chunk);

		public abstract int GetMinGenY();

		public abstract int GetGenHeight();

		public abstract int GetBaseHeight(int x, IHeightLimitView heightLimit);

		public int GetFirstFreeHeight(int x, IHeightLimitView heightLimit) {
			return this.GetBaseHeight(x, heightLimit);
		}

		public int GetFirstOccupiedHeight(int x, IHeightLimitView heightLimit) {
			return this.GetBaseHeight(x, heightLimit) - 1;
		}
	}
}
