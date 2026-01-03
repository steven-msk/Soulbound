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

namespace Assets.Scripts.Client.World.Biome {
	public class Biome_test2 : IBiome {
		const float maxSolidDepth = 10f;
		const int platformHeight = 0;

		private readonly int seed;
		private readonly Heightmap heightmap;
		private readonly PerlinNoise largeNoise;
		private readonly PerlinNoise mediumNoise;
		private readonly PerlinNoise detailNoise;
		private readonly DomainWarp warp;
		private readonly PerlinNoise densityNoise;
		int lastTreeX = int.MinValue >> 1;

		public Biome_test2(int seed) {
			this.largeNoise = new PerlinNoise(1, seed, frequency: 0.5f, amplitude: 100f);
			this.mediumNoise = new PerlinNoise(2, seed, frequency: 1.4f, amplitude: 40f);
			this.detailNoise = new PerlinNoise(3, seed, frequency: 0.06f, amplitude: 10f);
			this.warp = new DomainWarp(seed, frequency: 0.15f);
			this.densityNoise = new PerlinNoise(8, seed, frequency: 0.06f, amplitude: 1f);
		}

		float IBiome.GetDensity(int blockX) {
			float n = Mathf.Abs(densityNoise.Sample1D(blockX));
			n = Mathf.Pow(n, 6f);
			return n;
		}

		float IBiome.GetDepth(BlockPos pos) {
			float height = SurfaceDepthAtX(pos.x);
			float depth = height - pos.y;
			float normalized = Mathf.Clamp01(depth / maxSolidDepth);
			return normalized * maxSolidDepth;
		}

		private float SurfaceDepthAtX(int x) {
			float ln = Mathf.Abs(largeNoise.Sample1D(x));
			float mn = Mathf.Abs(mediumNoise.Sample1D(x));
			float dn = Mathf.Abs(detailNoise.Sample1D(x));
			return platformHeight + ln + mn + dn;
		}

		public BlockState ResolveBlock(float depth, BlockPos pos) {
			if (depth <= 0)
				return Blocks.air.defaultState;
			if (depth < 5) 
				return Blocks.dirt.defaultState;
			if (depth < 2)
				return Blocks.grass.defaultState;
			return Blocks.stone.defaultState;
		}
	}
}
