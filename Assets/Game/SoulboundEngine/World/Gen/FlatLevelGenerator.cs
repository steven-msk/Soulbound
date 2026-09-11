namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.Registry;
	using SoulboundEngine.Serialization;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Block.State;
	using SoulboundEngine.World.Chunk;
	using SoulboundEngine.World.Gen.Biome;
	using SoulboundEngine.World.Level;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class FlatLevelGenerator : ChunkGenerator {
		public new static readonly MapCodec<ChunkGenerator> CODEC = RecordMapCodec<ChunkGenerator, Settings>.Of(
			Field.Required<ChunkGenerator, Settings>("settings", Settings.CODEC , v => ((FlatLevelGenerator)v).settings),
			settings => new FlatLevelGenerator(settings)
		);
		private readonly Settings settings;
		[Obsolete] private static readonly List<Block> DEFAULT_LAYERS = new() {
			Blocks.STONE, Blocks.STONE, Blocks.STONE, Blocks.STONE,
			Blocks.DIRT
		};

		[Obsolete("Provisory plains biome default: forces WorldPreset registration to run after Biome registration due to entry lookup")]
		public FlatLevelGenerator() : this(new Settings(GetBiome(), DEFAULT_LAYERS)) { }

		private static RegistryEntry<Biome.Biome> GetBiome() {
			Logger.LogInfo(Registries.BIOME.Get(Biome.Biome.PLAINS));
			return Registries.BIOME.Get(Biome.Biome.PLAINS);
		}

		public FlatLevelGenerator(Settings settings)
			: base(new SingleBiomeSource(settings.biome)) {
			this.settings = settings;
		}

		protected override MapCodec<ChunkGenerator> Codec() => CODEC;

		public override Chunk Fill(RandomState randomState, Chunk chunk) {
			List<BlockState> layers = this.settings.layers.Select(b => b.DefaultState).ToList();
			Heightmap heightmap = chunk.GetHeightmap();
			BlockPos.Mutable blockPos = new();

			for (int layerIndex = 0; layerIndex < Math.Min(chunk.GetHeight(), layers.Count); layerIndex++) {
				BlockState blockState = layers[layerIndex];
				int y = chunk.GetBottomY() + layerIndex;

				for (int x = 0; x < Level.CHUNK_LENGTH; x++) {
					chunk.SetBlockState(blockPos.Set(x, y), blockState);
					heightmap.Update(x, y, blockState);
				}
			}

			return chunk;
		}

		public override Chunk BuildSurface(RandomState randomState, Chunk chunk) {
			return chunk;
		}

		public override int GetBaseHeight(int x, IHeightLimitView heightLimit) {
			List<BlockState> layers = this.settings.layers.Select(b => b.DefaultState).ToList();
			for (int layerIndex = Math.Min(layers.Count - 1, heightLimit.GetTopY()); layerIndex >= 0; layerIndex--) {
				BlockState state = layers[layerIndex];
				if (!state.IsAir()) {
					return heightLimit.GetBottomY() + layerIndex + 1;
				}
			}
			return heightLimit.GetBottomY();
		}

		public override int GetMinGenY() => 0;

		public override int GetGenHeight() => Level.DEFAULT_WORLD_HEIGHT;

		public Settings GetSettings() => this.settings;

		public sealed record Settings(RegistryEntry<Biome.Biome> biome, List<Block> layers) {
			public static readonly Codec<Settings> CODEC = RecordCodec<Settings, RegistryEntry<Biome.Biome>, List<Block>>.Of(
				Field.Required<Settings, RegistryEntry<Biome.Biome>>("biome", Biome.Biome.ENTRY_CODEC, s => s.biome),
				Field.Required<Settings, List<Block>>("blocks", Block.CODEC.ListOf(), s => s.layers),
				(biome, layers) => new Settings(biome, layers)
			);
		}
	}
}
