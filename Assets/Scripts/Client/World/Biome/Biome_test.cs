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

		public Biome_test(int seed, int platformHeight) {
			this.platformHeight = platformHeight;
			this.largeNoise = new PerlinNoise(seed, frequency: 1f, amplitude: 60f);
			this.mediumNoise = new PerlinNoise(seed, frequency: 0.7f, amplitude: 40f);
			this.detailNoise = new PerlinNoise(seed, frequency: 0.12f, amplitude: 5f);
			this.heightmap = new Heightmap(platformHeight);
			this.caveNoise = new PerlinNoise(seed + 1200, frequency: 0.5f, amplitude: 5f);
		}

		public float GetDensity(int x, int y) {
			const float maxSolidDepth = 10f;
			float ln = Mathf.Abs(largeNoise.Sample1D(x));
			float mn = Mathf.Abs(mediumNoise.Sample1D(x));
			float dn = Mathf.Abs(detailNoise.Sample1D(x));
			float height = platformHeight + ln + mn + dn;
			float depth = height - y;
			float normalizedDepth = Mathf.Clamp01(depth / maxSolidDepth);
			float density = normalizedDepth * maxSolidDepth;
			return density;
		}

		private bool IsCave(int x, int y) {
			float n = Mathf.Abs(caveNoise.Sample2D(x, y));
			return n > 0.65f;
		}

		public BlockState ResolveBlock(float density, int x, int y) {
			if (density <= 0 || IsCave(x, y))
				return Blocks.air.defaultState;
			if (density < 4)
				return Blocks.grass.defaultState;
			if (density < 1)
				return Blocks.dirt.defaultState;
			return Blocks.stone.defaultState;
		}
	}
}
