namespace SoulboundEngine.World.Biome {
	using SoulboundEngine.Common.Math;
	using SoulboundEngine.Common.Math.Noise;
	using SoulboundEngine.Common.Math.Random;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Block.State;
	using SoulboundEngine.World.Chunk;
	using SoulboundEngine.World.Gen;
	using SoulboundEngine.World.Level;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class HillsBiome : IBiome {
		private readonly int seed;
		private readonly Heightmap heightmap;
		private readonly NoiseSampler largeNoise;
		private readonly NoiseSampler mediumNoise;
		private readonly NoiseSampler densityNoise;
		private readonly NoiseSampler forestNoise;
		private readonly NoiseSampler forestDensityNoise;
		private readonly IRandom random;
		int lastTreeX = int.MinValue >> 1;

		public HillsBiome(int seed) {
			this.largeNoise = new NoiseSampler(1, new NoiseSettings(seed, NoiseType.Perlin, 0.01f));
			this.mediumNoise = new NoiseSampler(2, new NoiseSettings(seed, NoiseType.Perlin, 0.02f));
			this.densityNoise = new NoiseSampler(8, new NoiseSettings(seed, NoiseType.OpenSimplex2, 0.0012f));
			this.forestNoise = new NoiseSampler(6, new NoiseSettings(seed, NoiseType.Value, 0.03f));
			this.forestDensityNoise = new NoiseSampler(7, new NoiseSettings(seed, NoiseType.Value, 0.05f));
			this.random = new Xoshiro256StarStarRandom(seed);
		}

		float IBiome.GetDensity(int blockX) {
			float n = this.densityNoise.Sample1D(blockX);
			n = (n + 1f) * 0.5f;
			n = (float)Maths.SmoothStep(0f, 1f, n);
			n = (float)Math.Pow(n, 1.5f);
			return n;
		}

		private float HeightNoise(int x) {
			const float largeAmp = 100f;
			const float mediumAmp = 40f;

			float ln = (this.largeNoise.Sample1D(x) + 1f) * 0.5f * largeAmp;
			float mn = (this.mediumNoise.Sample1D(x) + 1f) * 0.5f * mediumAmp;
			return ln + mn;
		}

		BlockState IBiome.ResolveBlock(BlockGenContext ctx) {
			return ctx.AboveSurface()
				? Blocks.AIR.DefaultState
				: ctx.distanceToSurface < 2
					? Blocks.GRASS.DefaultState
					: ctx.distanceToSurface < 5 ? Blocks.DIRT.DefaultState : Blocks.STONE.DefaultState;
		}

		void PlaceTree(int originX, int originY, Chunk chunk, Level level) {
			const int crownRadius = 2;
			const int trunkHeightMin = 5;
			const int trunkHeightMax = 20;

			BlockPos trunkPos = new(originX, originY);
			int height = this.random.NextInt(trunkHeightMin, trunkHeightMax + 1);

			for (int y = 0; y < height; y++) {
				chunk.SetBlockState(trunkPos, Blocks.WOOD.DefaultState);
				trunkPos.y++;
			}

			Dictionary<int, List<int>> rowToXs = new();
			float angularStep = 1f;
			for (float angle = 0; angle < 360f; angle += angularStep) {
				float rad = angle * (float)Maths.DEG_2_RAD;
				int x = (int)Math.Round(trunkPos.x + crownRadius * Math.Cos(rad));
				int y = (int)Math.Round(trunkPos.y + crownRadius * Math.Sin(rad));

				if (!rowToXs.ContainsKey(y)) {
					rowToXs[y] = new List<int>();
				}
				rowToXs[y].Add(x);
			}

			foreach (KeyValuePair<int, List<int>> kvp in rowToXs) {
				int y = kvp.Key;
				List<int> xs = kvp.Value;
				for (int x = xs.Min(); x <= xs.Max(); x++) {
					BlockPos blockPos = new(x, y);
					level.SetBlockState(blockPos, Blocks.LEAVES.DefaultState);
				}
			}

		}

		void IBiome.PostProcess(ChunkGenData genData, Chunk chunk, Level level, int partitionStartX, int partitionLimitX) {
			const float chanceMin = 0.05f;
			const float chanceMax = 0.25f;
			const float threshold = 0.45f;
			const float minTreeSpacing = 3;
			const float forestAmp = 10f;
			const float densityAmp = 4f;

			for (int x = partitionStartX; x <= partitionLimitX; x++) {
				float forest = Math.Abs(this.forestNoise.Sample1D(x) * forestAmp);
				if (forest < threshold) {
					continue;
				}

				float density = Math.Abs(this.forestDensityNoise.Sample1D(x) * densityAmp);
				float distance = Math.Abs(x - this.lastTreeX);
				if (distance < minTreeSpacing) {
					continue;
				}

				float spawnChance = (float)Maths.Lerp(chanceMin, chanceMax, density);
				if (this.random.NextFloat() < spawnChance) {
					this.PlaceTree(x, genData.surfacePoints[chunk.GetPos().WorldXToChunkX(x)] + 1, chunk, level);
					this.lastTreeX = x;
				}
			}
		}

		TerrainModulation IBiome.SampleTerrain(int blockX) {
			return new TerrainModulation {
				heightOffset = 40f + this.HeightNoise(blockX),
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
			return Math.Abs(density) <= 0.05f ? Blocks.DIRT.DefaultState : Blocks.AIR.DefaultState;
		}
	}
}
