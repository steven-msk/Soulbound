namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.Serialization;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Block.State;
	using SoulboundEngine.World.Chunk;
	using SoulboundEngine.World.Level;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class FlatLevelGenerator : ChunkGenerator {
		public new static readonly MapCodec<ChunkGenerator> CODEC = RecordMapCodec<ChunkGenerator, Settings>.Of(
			Field.Required<ChunkGenerator, Settings>("settings", Settings.CODEC , v => ((FlatLevelGenerator)v).settings),
			settings => new FlatLevelGenerator(settings)
		);
		private static readonly List<Block> DEFAULT_LAYERS = new() {
			Blocks.STONE, Blocks.STONE, Blocks.STONE, Blocks.STONE,
			Blocks.DIRT
		};
		private readonly Settings settings;

		[Obsolete]
		public FlatLevelGenerator()
			: this(new Settings(DEFAULT_LAYERS)) {
		}

		public FlatLevelGenerator(Settings settings) {
			this.settings = settings;
		}

		protected override MapCodec<ChunkGenerator> Codec() => CODEC;

		public override Chunk Fill(Chunk chunk) {
			List<BlockState> layers = this.settings.layers.Select(b => b.DefaultState).ToList();
			BlockPos.Mutable blockPos = new();

			for (int layerIndex = 0; layerIndex < Math.Min(chunk.GetHeight(), layers.Count); layerIndex++) {
				BlockState blockState = layers[layerIndex];
				int y = chunk.GetBottomY() + layerIndex;

				for (int x = 0; x < Level.CHUNK_LENGTH; x++) {
					chunk.SetBlockState(blockPos.Set(x, y), blockState);
				}
			}

			return chunk;
		}

		public override Chunk GenerateSurface(Chunk chunk) {
			int[] heightmap = chunk.GetHeightmap();
			for (int i = 0; i < heightmap.Length; i++) {
				heightmap[i] = this.GetBaseHeight(i, chunk);
			}
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

		public sealed record Settings(List<Block> layers) {
			public static readonly Codec<Settings> CODEC = Block.CODEC.ListOf().Xmap(l => new Settings(l), s => s.layers);
		}
	}
}
