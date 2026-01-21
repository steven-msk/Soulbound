using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Client.World.Generation;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Client.World.Biome {
	public class HillsBiome_test : IBiome {
		private readonly int seed;
		private readonly Heightmap heightmap;
		private readonly NoiseSampler largeNoise;
		private readonly NoiseSampler mediumNoise;
		private readonly NoiseSampler densityNoise;
		private readonly NoiseSampler forestNoise;
		private readonly NoiseSampler forestDensityNoise;
		int lastTreeX = int.MinValue >> 1;

		public HillsBiome_test(int seed) {
			largeNoise = new NoiseSampler(1, seed, new(FastNoiseLite.NoiseType.Perlin, 0.01f));
			mediumNoise = new NoiseSampler(2, seed, new(FastNoiseLite.NoiseType.Perlin, 0.02f));
			densityNoise = new NoiseSampler(8, seed, new(FastNoiseLite.NoiseType.OpenSimplex2, 0.0012f));
			forestNoise = new NoiseSampler(6, seed, new(FastNoiseLite.NoiseType.Value, 0.03f));
			forestDensityNoise = new NoiseSampler(7, seed, new(FastNoiseLite.NoiseType.Value, 0.05f));
		}

		float IBiome.GetDensity(int blockX) {
			float n = densityNoise.Sample1D(blockX);
			n = (n + 1f) * 0.5f;
			n = Mathf.SmoothStep(0f, 1f, n);
			n = Mathf.Pow(n, 1.5f);
			return n;
		}

		private float HeightNoise(int x) {
			const float largeAmp = 100f;
			const float mediumAmp = 40f;

			float ln = (largeNoise.Sample1D(x) + 1f) * 0.5f * largeAmp;
			float mn = (mediumNoise.Sample1D(x) + 1f) * 0.5f * mediumAmp;
			return ln + mn;
		}

		BlockState IBiome.ResolveBlock(BlockGenContext ctx) {
			if (ctx.AboveSurface())
				return Blocks.air.defaultState;
			if (ctx.distanceToSurface < 2)
				return Blocks.grass.defaultState;
			if (ctx.distanceToSurface < 5)
				return Blocks.dirt.defaultState;
			return Blocks.stone.defaultState;
		}

		void PlaceTree(int originX, int originY, WorldChunk chunk, Level level) {
			const int crownRadius = 2;
			const int trunkHeightMin = 5;
			const int trunkHeightMax = 20;

			BlockPos trunkPos = new(originX, originY);
			int height = UnityEngine.Random.Range(trunkHeightMin, trunkHeightMax + 1);

			for (int y = 0; y < height; y++) {
				chunk.SetBlock(trunkPos.ToChunk(), Blocks.wood.defaultState);
				trunkPos.y++;
			}

			Dictionary<int, List<int>> rowToXs = new();
			float angularStep = 1f;
			for (float angle = 0; angle < 360f; angle += angularStep) {
				float rad = angle * Mathf.Deg2Rad;
				int x = Mathf.RoundToInt(trunkPos.x + crownRadius * Mathf.Cos(rad));
				int y = Mathf.RoundToInt(trunkPos.y + crownRadius * Mathf.Sin(rad));

				if (!rowToXs.ContainsKey(y)) {
					rowToXs[y] = new List<int>();
				}
				rowToXs[y].Add(x);
			}

			foreach (var kvp in rowToXs) {
				int y = kvp.Key;
				List<int> xs = kvp.Value;
				for (int x = xs.Min(); x <= xs.Max(); x++) {
					BlockPos blockPos = new(x, y);
					level.SetBlockOrPend(blockPos.ToChunk(), Blocks.leaves.defaultState);
				}
			}

		}

		void IBiome.PostProcess(ChunkGenData genData, WorldChunk chunk, Level level, int partitionStartX, int partitionLimitX) {
			const float chanceMin = 0.05f;
			const float chanceMax = 0.25f;
			const float threshold = 0.45f;
			const float minTreeSpacing = 3;
			const float forestAmp = 10f;
			const float densityAmp = 4f;

			for (int x = partitionStartX; x <= partitionLimitX; x++) {
				float forest = Mathf.Abs(forestNoise.Sample1D(x) * forestAmp);
				if (forest < threshold) {
					continue;
				}

				float density = Mathf.Abs(forestDensityNoise.Sample1D(x) * densityAmp);
				float distance = Mathf.Abs(x - lastTreeX);
				if (distance < minTreeSpacing) {
					continue;
				}

				float spawnChance = Mathf.Lerp(chanceMin, chanceMax, density);
				if (UnityEngine.Random.value < spawnChance) {
					PlaceTree(x, genData.surfacePoints[chunk.WorldXToChunkX(x)] + 1, chunk, level);
					lastTreeX = x;
				}
			}
		}

		TerrainModulation IBiome.SampleTerrain(int blockX) {
			return new TerrainModulation {
				heightOffset = 40f + HeightNoise(blockX),
				amplitude = 0.52f,
				erosion = 0.5f
			};
		}

		CaveModulation IBiome.SampleCave(int blockX, int blockY) {
			return new CaveModulation {
				frequency = 0.03f,
				sharpness = 2f,
				fill = 0.6f,
				lacunarity = 0.5f,
				octaves = 3,
				persistence = 0.1f,
				surfaceFalloff = 60f,
				bottomFalloff = 20f
			};
		}

		BlockState IBiome.ResolveCaveBlock(BlockPos pos, float density) {
			if (Mathf.Abs(density) <= 0.05f)
				return Blocks.dirt.defaultState;
			return Blocks.air.defaultState;
		}
	}
}
