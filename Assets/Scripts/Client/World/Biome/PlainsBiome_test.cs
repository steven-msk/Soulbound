using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Client.World.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using TerrainData = SoulboundBackend.Client.World.Generation.TerrainData;

namespace Assets.Scripts.Client.World.Biome {
	public class PlainsBiome_test : IBiome {
		private readonly int seed;
		private readonly NoiseSampler largeNoise;
		private readonly NoiseSampler mediumNoise;
		private readonly NoiseSampler densityNoise;

		public PlainsBiome_test(int seed) {
			largeNoise = new NoiseSampler(1, seed, new(FastNoiseLite.NoiseType.Perlin, 0.007f));
			mediumNoise = new NoiseSampler(2, seed, new(FastNoiseLite.NoiseType.Perlin, 0.01f));
			densityNoise = new NoiseSampler(8, seed, new(FastNoiseLite.NoiseType.OpenSimplex2, 0.001f));
		}

		float IBiome.GetDensity(int blockX) {
			float n = densityNoise.Sample1D(blockX);
			n = (n + 1f) * 0.5f;
			n = Mathf.SmoothStep(0f, 1f, n);
			n = Mathf.Pow(n, 1.5f);
			return n;
		}

		private float HeightNoise(int x) {
			const float largeAmp = 30f;
			const float mediumAmp = 20f;

			float ln = (largeNoise.Sample1D(x) + 1f) * 0.5f * largeAmp;
			float mn = (mediumNoise.Sample1D(x) + 1f) * 0.5f * mediumAmp;
			return ln + mn;
		}

		BlockState IBiome.ResolveBlock(BlockContext ctx) {
			if (ctx.AboveSurface())
				return Blocks.air.defaultState;
			return Blocks.dirt.defaultState;
		}

		TerrainModulation IBiome.SampleTerrain(int blockX) {
			return new TerrainModulation {
				heightOffset = 30 + HeightNoise(blockX),
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
