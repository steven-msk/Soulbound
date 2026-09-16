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
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using UnityEngine.Tilemaps;

	public class EditorWorldGenTool {
		private const int TILES_PER_BATCH = (Level.CHUNK_LENGTH * Level.DEFAULT_WORLD_HEIGHT) >> 6;

		public static async UniTask Regenerate(int maxConcurrentChunks, long seed, int chunkCount, int chunkStartX, Dictionary<RegistryKey<NormalNoise.Parameters>, NormalNoise.Parameters> overrides, EditorWorldGenContext context) {
			BlockRenderManager blockRenderManager = new(Registries.BLOCK.ToList());

			NoiseLevelChunkGenerator chunkGenerator = context.CreateChunkGenerator(NoiseGeneratorSettings.DEFAULT, MultiNoiseBiomeSourceParamList.DEFAULT);
			RandomState randomState = context.CreateRandomStateWithOverrides(NoiseGeneratorSettings.DEFAULT, seed, overrides);
			IHeightLimitView heightLimit = IHeightLimitView.Create(Level.DEFAULT_MIN_Y, Level.DEFAULT_WORLD_HEIGHT);
			Tilemap tilemap = UnityEngine.Object.FindFirstObjectByType<Tilemap>();
			tilemap.ClearAllTiles();
			Stopwatch stopwatch = Stopwatch.StartNew();

			List<UniTask> tasks = new();
			ConcurrentBag<UniTask> renderTasks = new();
			GenTime genTime = new();
			object taskLock = new();
			int activeTasks = 0;

			int i = 0;
			while (i < chunkCount) {
				while (activeTasks < maxConcurrentChunks && i < chunkCount) {
					await UniTask.Yield(PlayerLoopTiming.Update);
					ChunkPos pos = new(i + chunkStartX);
					Chunk chunk = new EditorWorldGenChunk(pos, heightLimit, () => new BlockStateContainer(ChunkSection.WIDTH, ChunkSection.HEIGHT));
					UniTask<Chunk> genTask = GenerateAsync(i, randomState, chunk, chunkGenerator, elapsed => {
						lock (taskLock) {
							genTime += elapsed;
							activeTasks--;
						}
					});
					tasks.Add(genTask.ContinueWith(chunk => {
						renderTasks.Add(RenderChunk(chunkStartX, TILES_PER_BATCH, tilemap, chunk, blockRenderManager));
					}));

					activeTasks++;
					i++;
				}
				await UniTask.Yield(PlayerLoopTiming.Update);
			}

			await UniTask.WhenAll(tasks);
			stopwatch.Stop();
			await UniTask.WhenAll(renderTasks);
			SoulboundEngine.Logger.LogWarning(
				"Finished generating {} chunks in {}ms cumulative CPU time ({}ms/chunk: {}ms biomes, {}ms fill, {}ms surface)",
				chunkCount, stopwatch.ElapsedMilliseconds, 
				(double)genTime.total / chunkCount,
				(double)genTime.biomes / chunkCount,
				(double)genTime.fill / chunkCount,
				(double)genTime.surface / chunkCount
			);
		}

		private static async UniTask<Chunk> GenerateAsync(
			int index,
			RandomState randomState,
			Chunk chunk,
			ChunkGenerator chunkGenerator,
			Action<GenTime> reportDone
		) {
			(Chunk generated, GenTime elapsed) = await UniTask.RunOnThreadPool(() => {
				Stopwatch stopwatch = Stopwatch.StartNew();
				Chunk result = chunkGenerator.MapBiomes(randomState, chunk);
				long biomes = stopwatch.ElapsedMilliseconds;

				stopwatch.Restart();
				result = chunkGenerator.Fill(randomState, result);
				long fill = stopwatch.ElapsedMilliseconds;

				stopwatch.Restart();
				result = chunkGenerator.BuildSurface(randomState, result);
				long surface = stopwatch.ElapsedMilliseconds;

				return (result, new GenTime {
					total = biomes + fill + surface,
					biomes = biomes,
					fill = fill,
					surface = surface
				});
			});

			await UniTask.SwitchToMainThread(PlayerLoopTiming.Update);
			SoulboundEngine.Logger.LogWarning("Chunk {} took {}ms to generate ({}ms biomes, {}ms fill, {}ms surface)", 
				index, elapsed.total, elapsed.biomes, elapsed.fill, elapsed.surface);
			reportDone(elapsed);
			return generated;
		}

		public static async UniTask RenderChunk(int chunkStartX, int tilesPerBatch, Tilemap tilemap, Chunk chunk, BlockRenderManager blockRenderManager) {
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

		public struct GenTime {
			public long total;
			public long biomes;
			public long fill;
			public long surface;

			public static GenTime operator +(GenTime a, GenTime b) {
				return new GenTime {
					total = a.total + b.total,
					biomes = a.biomes + b.biomes,
					fill = a.fill + b.fill,
					surface = a.surface + b.surface
				};
			}
		}
	}
}
