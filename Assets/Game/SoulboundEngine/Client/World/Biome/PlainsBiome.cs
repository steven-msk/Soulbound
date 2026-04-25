using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Client.World.BlockSystem.States;
using UnityEngine;

namespace SoulboundEngine.Client.World.Generation {
	public class PlainsBiome : IBiome {
		private readonly NoiseSampler largeNoise;
		private readonly NoiseSampler mediumNoise;
		private readonly NoiseSampler densityNoise;

		public PlainsBiome(int seed) {
			this.largeNoise = new NoiseSampler(1, seed, new(FastNoiseLite.NoiseType.Perlin, 0.007f));
			this.mediumNoise = new NoiseSampler(2, seed, new(FastNoiseLite.NoiseType.Perlin, 0.01f));
			this.densityNoise = new NoiseSampler(8, seed, new(FastNoiseLite.NoiseType.OpenSimplex2, 0.001f));
		}

		float IBiome.GetDensity(int blockX) {
			float n = this.densityNoise.Sample1D(blockX);
			n = (n + 1f) * 0.5f;
			n = Mathf.SmoothStep(0f, 1f, n);
			n = Mathf.Pow(n, 1.5f);
			return n;
		}

		private float HeightNoise(int x) {
			const float largeAmp = 30f;
			const float mediumAmp = 20f;

			float ln = (this.largeNoise.Sample1D(x) + 1f) * 0.5f * largeAmp;
			float mn = (this.mediumNoise.Sample1D(x) + 1f) * 0.5f * mediumAmp;
			return ln + mn;
		}

		BlockState IBiome.ResolveBlock(BlockGenContext ctx) {
			if (ctx.AboveSurface())
				return Blocks.air.DefaultState;
			return Blocks.dirt.DefaultState;
		}

		TerrainModulation IBiome.SampleTerrain(int blockX) {
			return new TerrainModulation {
				heightOffset = 30 + this.HeightNoise(blockX),
				amplitude = 0.35f,
				erosion = 0.85f
			};
		}

		CaveModulation IBiome.SampleCave(int blockX, int blockY) {
			return new CaveModulation {
				frequency = 0.008f,
				sharpness = 1.5f,
				fill = 0.35f,
				octaves = 3,
				lacunarity = 2f,
				persistence = 0.45f,
				surfaceFalloff = 30f,
				bottomFalloff = 10f,
				warpFrequency = 0.004f,
				warpAmp = 5f
			};
		}
	}
}
