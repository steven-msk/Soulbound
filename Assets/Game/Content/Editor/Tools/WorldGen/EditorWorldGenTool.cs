namespace SoulboundEngine.UnityClient.Editor.Tools.WorldGen {
	using SoulboundEngine.Registry;
	using SoulboundEngine.UnityClient.Render.Block;
	using SoulboundEngine.World;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Chunk;
	using SoulboundEngine.World.Gen;
	using SoulboundEngine.World.Gen.Biome;
	using SoulboundEngine.World.Gen.Noise;
	using SoulboundEngine.World.Level;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.Tilemaps;

	public class EditorWorldGenTool {
		public static void Regenerate(Dictionary<RegistryKey<NormalNoise.Parameters>, NormalNoise.Parameters> overrides, EditorWorldGenContext context) {
			BlockRenderManager blockRenderManager = new(Registries.BLOCK.ToList());

			NoiseLevelChunkGenerator chunkGenerator = context.CreateChunkGenerator(NoiseGeneratorSettings.DEFAULT, MultiNoiseBiomeSourceParamList.DEFAULT);
			Chunk chunk = new EditorWorldGenChunk(ChunkPos.ORIGIN, IHeightLimitView.Create(Level.DEFAULT_MIN_Y, Level.DEFAULT_WORLD_HEIGHT), () => new BlockStateContainer(ChunkSection.WIDTH, ChunkSection.HEIGHT));

			RandomState randomState = context.CreateRandomStateWithOverrides(NoiseGeneratorSettings.DEFAULT, 0L, overrides);
			chunk = chunkGenerator.MapBiomes(randomState, chunk);
			chunk = chunkGenerator.Fill(randomState, chunk);
			chunk = chunkGenerator.BuildSurface(randomState, chunk);

			RenderChunk(chunk, blockRenderManager);
		}

		public static void RenderChunk(Chunk chunk, BlockRenderManager blockRenderManager) {
			BlockPos.Mutable blockPos = new();
			Tilemap tilemap = Object.FindFirstObjectByType<Tilemap>();
			for (int x = 0; x < Level.CHUNK_LENGTH; x++) {
				for (int y = Level.DEFAULT_MIN_Y; y <= Level.DEFAULT_MAX_Y; y++) {
					blockRenderManager.Render(tilemap, x, y, chunk.GetBlockState(blockPos.Set(x, y)));
				}
			}
		}
	}
}
