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
		public static void Regenerate(long seed, int chunkCount, Dictionary<RegistryKey<NormalNoise.Parameters>, NormalNoise.Parameters> overrides, EditorWorldGenContext context) {
			BlockRenderManager blockRenderManager = new(Registries.BLOCK.ToList());

			NoiseLevelChunkGenerator chunkGenerator = context.CreateChunkGenerator(NoiseGeneratorSettings.DEFAULT, MultiNoiseBiomeSourceParamList.DEFAULT);
			RandomState randomState = context.CreateRandomStateWithOverrides(NoiseGeneratorSettings.DEFAULT, seed, overrides);
			IHeightLimitView heightLimit = IHeightLimitView.Create(Level.DEFAULT_MIN_Y, Level.DEFAULT_WORLD_HEIGHT);
			Tilemap tilemap = Object.FindFirstObjectByType<Tilemap>();
			tilemap.ClearAllTiles();
			for (int i = 0; i < chunkCount; i++) {
				ChunkPos pos = new(i);
				Chunk chunk = new EditorWorldGenChunk(pos, heightLimit, () => new BlockStateContainer(ChunkSection.WIDTH, ChunkSection.HEIGHT));

				chunk = chunkGenerator.MapBiomes(randomState, chunk);
				chunk = chunkGenerator.Fill(randomState, chunk);
				chunk = chunkGenerator.BuildSurface(randomState, chunk);

				RenderChunk(tilemap, chunk, blockRenderManager);
			}
		}

		public static void RenderChunk(Tilemap tilemap, Chunk chunk, BlockRenderManager blockRenderManager) {
			BlockPos.Mutable blockPos = new();
			for (int x = 0; x < Level.CHUNK_LENGTH; x++) {
				for (int y = Level.DEFAULT_MIN_Y; y <= Level.DEFAULT_MAX_Y; y++) {
					int worldX = x + Level.CHUNK_LENGTH * chunk.GetPos().x;
					blockRenderManager.Render(tilemap, worldX, y, chunk.GetBlockState(blockPos.Set(x, y)));
				}
			}
		}
	}
}
