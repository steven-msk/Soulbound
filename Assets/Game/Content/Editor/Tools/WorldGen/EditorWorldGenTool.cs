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
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using UnityEngine.Tilemaps;

	public class EditorWorldGenTool {
		private const int TILES_PER_BATCH = (Level.CHUNK_LENGTH * Level.DEFAULT_WORLD_HEIGHT) >> 6;
		private const int MAX_CONCURRENT_CHUNKS = 10;

		public static async UniTask Regenerate(long seed, int chunkCount, int chunkStartX, Dictionary<RegistryKey<NormalNoise.Parameters>, NormalNoise.Parameters> overrides, EditorWorldGenContext context) {
			BlockRenderManager blockRenderManager = new(Registries.BLOCK.ToList());

			NoiseLevelChunkGenerator chunkGenerator = context.CreateChunkGenerator(NoiseGeneratorSettings.DEFAULT, MultiNoiseBiomeSourceParamList.DEFAULT);
			RandomState randomState = context.CreateRandomStateWithOverrides(NoiseGeneratorSettings.DEFAULT, seed, overrides);
			IHeightLimitView heightLimit = IHeightLimitView.Create(Level.DEFAULT_MIN_Y, Level.DEFAULT_WORLD_HEIGHT);
			Tilemap tilemap = UnityEngine.Object.FindFirstObjectByType<Tilemap>();
			tilemap.ClearAllTiles();
			Stopwatch stopwatch = Stopwatch.StartNew();

			List<UniTask> tasks = new();
			long total = 0L;
			object taskLock = new();
			int activeTasks = 0;

			int i = 0;
			while (i < chunkCount) {
				while (activeTasks < MAX_CONCURRENT_CHUNKS && i < chunkCount) {
					await UniTask.Yield(PlayerLoopTiming.Update);
					ChunkPos pos = new(i + chunkStartX);
					Chunk chunk = new EditorWorldGenChunk(pos, heightLimit, () => new BlockStateContainer(ChunkSection.WIDTH, ChunkSection.HEIGHT));
					UniTask task = GenerateAndRenderAsync(i, chunkStartX, randomState, chunk, chunkGenerator, tilemap, blockRenderManager, elapsed => {
						lock (taskLock) { 
							total += elapsed;
							activeTasks--;
						}
					}, TILES_PER_BATCH);
					tasks.Add(task);

					activeTasks++;
					i++;
				}
				await UniTask.Yield(PlayerLoopTiming.Update);
			}

			await UniTask.WhenAll(tasks);
			SoulboundEngine.Logger.LogWarning("Finished generating {} chunks in {} total ms ({}ms average)", chunkCount, total, (double)total / chunkCount);
		}

		private static async UniTask GenerateAndRenderAsync(
			int index,
			int chunkStartX,
			RandomState randomState,
			Chunk chunk,
			ChunkGenerator chunkGenerator,
			Tilemap tilemap,
			BlockRenderManager blockRenderManager,
			Action<long> reportDone,
			int tilesPerBatch
		) {
			(Chunk generated, long elapsed) = await UniTask.RunOnThreadPool(() => {
				Stopwatch stopwatch = Stopwatch.StartNew();
				Chunk result = chunkGenerator.MapBiomes(randomState, chunk);
				result = chunkGenerator.Fill(randomState, result);
				result = chunkGenerator.BuildSurface(randomState, result);
				return (result, stopwatch.ElapsedMilliseconds);
			});

			await UniTask.SwitchToMainThread(PlayerLoopTiming.Update);
			SoulboundEngine.Logger.LogWarning("Chunk {} took {}ms to generate", index, elapsed);
			RenderChunk(chunkStartX, tilesPerBatch, tilemap, generated, blockRenderManager);
			reportDone(elapsed);
		}

		public static async void RenderChunk(int chunkStartX, int tilesPerBatch, Tilemap tilemap, Chunk chunk, BlockRenderManager blockRenderManager) {
			BlockPos.Mutable blockPos = new();
			int counter = 0;
			for (int y = chunk.GetBottomY(); y <= chunk.GetTopY(); y++) {
				for (int x = 0; x < Level.CHUNK_LENGTH; x++) {
					int tilemapX = Level.CHUNK_LENGTH * (chunk.GetPos().x - chunkStartX) + x;
					int worldX = chunk.GetPos().ToWorldX(x);
					blockRenderManager.Render(tilemap, tilemapX, y, chunk.GetBlockState(blockPos.Set(worldX, y)));

					if (++counter % tilesPerBatch == 0) {
						await UniTask.Yield(PlayerLoopTiming.Update);
					}
				}
			}
		}
	}
}
