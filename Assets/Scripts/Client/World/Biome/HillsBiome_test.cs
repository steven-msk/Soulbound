using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Client.World.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TerrainData = SoulboundBackend.Client.World.Generation.TerrainData;

namespace Assets.Scripts.Client.World.Biome {
	public class HillsBiome_test : IBiome {
		private readonly int seed;
		private readonly Heightmap heightmap;
		private readonly PerlinNoise largeNoise;
		private readonly PerlinNoise mediumNoise;
		private readonly PerlinNoise densityNoise;
		private readonly PerlinNoise forestNoise;
		private readonly PerlinNoise forestDensityNoise;
		int lastTreeX = int.MinValue >> 1;

		public HillsBiome_test(int seed) {
			this.largeNoise = new PerlinNoise(1, seed, frequency: 0.5f, amplitude: 100f);
			this.mediumNoise = new PerlinNoise(2, seed, frequency: 1.4f, amplitude: 40f);
			this.densityNoise = new PerlinNoise(8, seed, frequency: 0.06f, amplitude: 1f);
			this.forestNoise = new PerlinNoise(6, seed, frequency: 3f, amplitude: 10f);
			this.forestDensityNoise = new PerlinNoise(7, seed, frequency: 5f, amplitude: 4f);
		}

		float IBiome.GetDensity(int blockX) {
			float n = Mathf.Abs(densityNoise.Sample1D(blockX));
			n = Mathf.Pow(n, 1.5f);
			return n;
		}

		private float HeightNoise(int x) {
			float ln = Mathf.Abs(largeNoise.Sample1D(x));
			float mn = Mathf.Abs(mediumNoise.Sample1D(x));
			return ln + mn;
		}

		BlockState IBiome.ResolveBlock(BlockContext ctx) {
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

			int chunkX = chunk.WorldXToChunkX(originX), ypos = originY;
			ChunkBlockPos trunkPos = new(chunkX, ypos, chunk.xpos);
			int height = UnityEngine.Random.Range(trunkHeightMin, trunkHeightMax + 1);

			for (int y = 0; y < height; y++) {
				chunk.SetBlock(trunkPos, Blocks.wood.defaultState);
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
					int cx = level.ChunkXFromRelativeBlock(x, chunk.xpos);
					ChunkBlockPos pos = new(level.NormalizeChunkX(x), y, cx);
					level.SetBlockOrPend(pos, Blocks.leaves.defaultState);
				}
			}

		}

		void IBiome.PostProcessTerrain(TerrainData data, WorldChunk chunk, Level level, IEnumerable<BiomeInterval> intervals) {
			const float forestThreshold = 0.45f;
			const float minTreeSpacing = 3;

			foreach (var interval in intervals) {
				for (int x = interval.startXInclusive; x < interval.endXExclusive; x++) {
					float forest = Mathf.Abs(forestNoise.Sample1D(x));
					if (forest < forestThreshold) {
						continue;
					}

					float density = Mathf.Abs(forestDensityNoise.Sample1D(x));
					float distance = Mathf.Abs(x - lastTreeX);
					if (distance < minTreeSpacing) {
						continue;
					}

					float spawnChance = Mathf.Lerp(0.05f, 0.25f, density);
					if (UnityEngine.Random.value < spawnChance) {
						PlaceTree(x, data.surfacePoints[x] + 1, chunk, level);
						lastTreeX = x;
					}
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
				frequency = 3f,
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
