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
		private readonly PerlinNoise largeNoise;
		private readonly PerlinNoise mediumNoise;
		private readonly PerlinNoise densityNoise;

		public PlainsBiome_test(int seed) {
			this.largeNoise = new PerlinNoise(1, seed, frequency: 0.3f, amplitude: 30f);
			this.mediumNoise = new PerlinNoise(2, seed, frequency: 0.1f, amplitude: 20f);
			this.densityNoise = new PerlinNoise(8, seed, frequency: 0.05f, amplitude: 1f);
		}

		float IBiome.GetDensity(int blockX) {
			float n = densityNoise.Sample1D(blockX) * 0.5f + 0.5f;
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
				frequency = 1f,
				edgeSharpness = 1.5f,
				fill = 0.5f,
				surfaceFalloff = 30f,
				bottomFalloff = 10f
			};
		}
	}
}
