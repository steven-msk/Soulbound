namespace SoulboundEngine.UnityClient.Editor.Tools.WorldGen {
    using Cysharp.Threading.Tasks;
    using SoulboundEngine.Registry;
    using SoulboundEngine.UnityClient.Render.Block;
    using SoulboundEngine.World;
    using SoulboundEngine.World.Block;
    using SoulboundEngine.World.Chunk;
    using SoulboundEngine.World.Gen;
    using SoulboundEngine.World.Gen.Biome;
    using SoulboundEngine.World.Gen.Generator;
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

        public static async UniTask Regenerate(
            IWorldGenLevel level,
            int targetState, 
            int maxConcurrentChunks, 
            int chunkStartX,
            Dictionary<RegistryKey<NormalNoise.Parameters>, NormalNoise.Parameters> overrides, 
            EditorWorldGenContext context
        ) {
            BlockRenderManager blockRenderManager = new(Registries.BLOCK.ToList());

            NoiseLevelChunkGenerator chunkGenerator = context.CreateChunkGenerator(NoiseGeneratorSettings.DEFAULT, MultiNoiseBiomeSourceParamList.DEFAULT);
            RandomState randomState = context.CreateRandomStateWithOverrides(NoiseGeneratorSettings.DEFAULT, level.GetSeed(), overrides);
            Tilemap tilemap = UnityEngine.Object.FindFirstObjectByType<Tilemap>();
            tilemap.ClearAllTiles();
            Stopwatch stopwatch = Stopwatch.StartNew();

            List<UniTask> tasks = new();
            ConcurrentBag<UniTask> renderTasks = new();
            GenTime genTime = new();
            object taskLock = new();
            int activeTasks = 0;

            int i = 0;
            int chunkCount = level.GetChunkManager().GetLoadedChunkCount();
            while (i < chunkCount) {
                while (activeTasks < maxConcurrentChunks && i < chunkCount) {
                    await UniTask.Yield(PlayerLoopTiming.Update);
					int chunkPos = i + chunkStartX;
                    Chunk chunk = level.GetChunk(chunkPos, false);
                    UniTask<Chunk> genTask = GenerateAsync(level, i, randomState, chunk, chunkGenerator, targetState, elapsed => {
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
                "Finished generating {} chunks in {}ms cumulative CPU time ({}ms/chunk: {}ms biomes, {}ms fill, {}ms surface, {}ms features)",
                chunkCount, stopwatch.ElapsedMilliseconds, 
                (double)genTime.total / chunkCount,
                (double)genTime.biomes / chunkCount,
                (double)genTime.fill / chunkCount,
                (double)genTime.surface / chunkCount,
                (double)genTime.features / chunkCount
            );
        }

        private static async UniTask<Chunk> GenerateAsync(
            IWorldGenLevel level,
            int index,
            RandomState randomState,
            Chunk chunk,
            ChunkGenerator chunkGenerator,
            int targetState,
            Action<GenTime> reportDone
        ) {
            (Chunk generated, GenTime elapsed) = await UniTask.RunOnThreadPool(() => {
                Stopwatch stopwatch = Stopwatch.StartNew();
                Chunk result = chunkGenerator.MapBiomes(randomState, chunk);
                long biomes = stopwatch.ElapsedMilliseconds;

                stopwatch.Restart();
                if (targetState > 1) {
                    result = chunkGenerator.Fill(randomState, result);
                }
                long fill = stopwatch.ElapsedMilliseconds;

                stopwatch.Restart();
                if (targetState > 2) {
                    result = chunkGenerator.BuildSurface(randomState, result);
                }
                long surface = stopwatch.ElapsedMilliseconds;

                stopwatch.Restart();
                if (targetState > 3) {
                    result = chunkGenerator.ApplyDecor(level, randomState, result);
                }
                long features = stopwatch.ElapsedMilliseconds;

                return (result, new GenTime(biomes, fill, surface, features));
            });

            await UniTask.SwitchToMainThread(PlayerLoopTiming.Update);
            SoulboundEngine.Logger.LogWarning("Chunk {} took {}ms to generate ({}ms biomes, {}ms fill, {}ms surface, {}ms features)", 
                index, elapsed.total, elapsed.biomes, elapsed.fill, elapsed.surface, elapsed.features);
            reportDone(elapsed);
            return generated;
        }

        public static async UniTask RenderChunk(int chunkStartX, int tilesPerBatch, Tilemap tilemap, Chunk chunk, BlockRenderManager blockRenderManager) {
            BlockPos.Mutable blockPos = new();
            int counter = 0;
            for (int y = chunk.GetBottomY(); y <= chunk.GetTopY(); y++) {
                for (int x = 0; x < Level.CHUNK_LENGTH; x++) {
                    int tilemapX = Level.CHUNK_LENGTH * (chunk.pos.x - chunkStartX) + x;
                    int worldX = chunk.pos.ToWorldX(x);
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
            public long features;

            public GenTime(long biomes, long fill, long surface, long features) {
                this.biomes = biomes;
                this.fill = fill;
                this.surface = surface;
                this.features = features;
                this.total = biomes + fill + surface + features;
            }

            public static GenTime operator +(GenTime a, GenTime b) {
                return new GenTime(
                    biomes: a.biomes + b.biomes,
                    fill: a.fill + b.fill,
                    surface: a.surface + b.surface,
                    features: a.features + b.features
                );
            }
        }
    }
}
