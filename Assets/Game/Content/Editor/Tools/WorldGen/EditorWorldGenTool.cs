namespace SoulboundEngine.UnityClient.Editor.Tools.WorldGen {
	using Cysharp.Threading.Tasks;
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
	using System.Diagnostics;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.Tilemaps;

	public class EditorWorldGenTool {
		public static async UniTask Regenerate(long seed, int chunkCount, int chunkStartX, Dictionary<RegistryKey<NormalNoise.Parameters>, NormalNoise.Parameters> overrides, EditorWorldGenContext context) {
			BlockRenderManager blockRenderManager = new(Registries.BLOCK.ToList());

			NoiseLevelChunkGenerator chunkGenerator = context.CreateChunkGenerator(NoiseGeneratorSettings.DEFAULT, MultiNoiseBiomeSourceParamList.DEFAULT);
			RandomState randomState = context.CreateRandomStateWithOverrides(NoiseGeneratorSettings.DEFAULT, seed, overrides);
			IHeightLimitView heightLimit = IHeightLimitView.Create(Level.DEFAULT_MIN_Y, Level.DEFAULT_WORLD_HEIGHT);
			Tilemap tilemap = Object.FindFirstObjectByType<Tilemap>();
			tilemap.ClearAllTiles();
			Stopwatch stopwatch = Stopwatch.StartNew();

			List<UniTask<(Chunk chunk, long elapsed)>> tasks = new();

			for (int i = 0; i < chunkCount; i++) {
				ChunkPos pos = new(i + chunkStartX);
				Chunk chunk = new EditorWorldGenChunk(
					pos,
					heightLimit,
					() => new BlockStateContainer(
						ChunkSection.WIDTH,
						ChunkSection.HEIGHT
					)
				);
				tasks.Add(GenerateChunkAsync(randomState, chunk, chunkGenerator));
			}

			(Chunk chunk, long elapsed)[] results = await UniTask.WhenAll(tasks);
			long total = 0L;
			double average = 0.0d;
			for (int i = 0; i < results.Length; i++) {
				SoulboundEngine.Logger.LogWarning("Chunk {} took {}ms to generate", i, results[i].elapsed);
				average += (double)results[i].elapsed / chunkCount;
				total += results[i].elapsed;
				RenderChunk(chunkStartX, tilemap, results[i].chunk, blockRenderManager);
			}
			SoulboundEngine.Logger.LogWarning("Finished generating {} chunks in {} total ms ({}ms average)", chunkCount, total, average);
		}

		private static UniTask<(Chunk chunk, long elapsed)> GenerateChunkAsync(
			RandomState randomState,
			Chunk chunk,
			ChunkGenerator chunkGenerator
		) {
			return UniTask.RunOnThreadPool(() => {
				Stopwatch stopwatch = Stopwatch.StartNew();

				chunk = chunkGenerator.MapBiomes(randomState, chunk);
				chunk = chunkGenerator.Fill(randomState, chunk);
				chunk = chunkGenerator.BuildSurface(randomState, chunk);

				return (chunk, stopwatch.ElapsedMilliseconds);
			});
		}

		public static void RenderChunk(int chunkStartX, Tilemap tilemap, Chunk chunk, BlockRenderManager blockRenderManager) {
			BlockPos.Mutable blockPos = new();
			for (int x = 0; x < Level.CHUNK_LENGTH; x++) {
				for (int y = chunk.GetBottomY(); y <= chunk.GetTopY(); y++) {
					int tilemapX = Level.CHUNK_LENGTH * (chunk.GetPos().x - chunkStartX) + x;
					int worldX = chunk.GetPos().ToWorldX(x);
					blockRenderManager.Render(tilemap, tilemapX, y, chunk.GetBlockState(blockPos.Set(worldX, y)));
				}
			}
		}
	}
}
