using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Client.World.Biome {
	public class Biome_test : IBiome {
		private readonly int platformHeight;

		public Biome_test(int platformHeight) {
			this.platformHeight = platformHeight;
		}

		public IDensityGenerator CreateDensityGenerator(int colX) {
			const int seed = 94839204;

			return new SimpleHeightmapColumnGenerator(this, platformHeight, new PerlinNoise(seed, 0.01f, 100f));
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
