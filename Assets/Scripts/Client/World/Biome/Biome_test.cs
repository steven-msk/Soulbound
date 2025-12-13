using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Client.World.Biome {
	public class Biome_test : IBiome {
		private readonly int platformHeight;
		private readonly int seed;
		private readonly Heightmap heightmap;
		private readonly PerlinNoise largeNoise;
		private readonly PerlinNoise mediumNoise;
		private readonly PerlinNoise detailNoise;
		private readonly PerlinNoise caveNoise;
		private readonly DomainWarp warp;

		public Biome_test(int seed, int platformHeight) {
			this.platformHeight = platformHeight;
			this.largeNoise = new PerlinNoise(seed, frequency: 1f, amplitude: 60f);
			this.mediumNoise = new PerlinNoise(seed, frequency: 0.7f, amplitude: 40f);
			this.detailNoise = new PerlinNoise(seed, frequency: 0.12f, amplitude: 5f);
			this.heightmap = new Heightmap(platformHeight);
			this.caveNoise = new PerlinNoise(seed, frequency: 0.5f, amplitude: 1f);
			this.warp = new DomainWarp(seed, frequency: 0.125f);
		}

		public float GetDepth(int x, int y) {
			const float maxSolidDepth = 10f;
			float ln = Mathf.Abs(largeNoise.Sample1D(x));
			float mn = Mathf.Abs(mediumNoise.Sample1D(x));
			float dn = Mathf.Abs(detailNoise.Sample1D(x));
			float height = platformHeight + ln + mn + ln;
			float depth = height - y;
			float normalized = Mathf.Clamp01(depth / maxSolidDepth);
			return normalized * maxSolidDepth;
		}

		private bool IsCave(int x, int y) {
			float n = Mathf.Abs(caveNoise.Sample2D(x, y));
			return n > 0.65f;
		}

		private bool IsTunnel(int x, int y) {
			float wx = x, wy = y;
			warp.Warp2D(ref wx, ref wy);
			float n = 1f - Mathf.Abs(caveNoise.Sample2D(x + wx, y + wy));
			return n > 0.95f;
		}

		public BlockState ResolveBlock(float depth, int x, int y) {
			if (depth <= 0 || IsCave(x, y) || IsTunnel(x, y))
				return Blocks.air.defaultState;
			if (depth < 4)
				return Blocks.grass.defaultState;
			if (depth < 1)
				return Blocks.dirt.defaultState;
			return Blocks.stone.defaultState;
		}
	}
}
