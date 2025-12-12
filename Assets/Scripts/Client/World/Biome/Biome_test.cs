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

		public Biome_test(int seed, int platformHeight) {
			this.platformHeight = platformHeight;
			this.largeNoise = new PerlinNoise(seed, frequency: 0.008f, amplitude: 90f);
			this.mediumNoise = new PerlinNoise(seed, frequency: 0.003f, amplitude: 65f); 
			this.detailNoise = new PerlinNoise(seed, frequency: 0.12f, amplitude: 5f);
			this.heightmap = new Heightmap(platformHeight);
		}

		public float GetDensity(int x, int y) {
			float ln = largeNoise.Sample1D(x) * 2f - 1f;
			float mn = mediumNoise.Sample1D(x) * 2f - 1f;
			float dn = detailNoise.Sample1D(x) * 2f - 1f;
			float height = ln + mn + dn - platformHeight;
			return height - y;
		}

		public BlockState ResolveBlock(float density, int x, int y) {
			if (density <= 0)
				return Blocks.air.defaultState;
			if (density < 4)
				return Blocks.grass.defaultState;
			if (density < 1)
				return Blocks.dirt.defaultState;
			return Blocks.stone.defaultState;
		}
	}
}
