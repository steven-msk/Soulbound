using SoulboundBackend.Client.World.BlockSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.Generation {
	public class SimpleHeightmapColumnGenerator : IColumnGenerator {
		private readonly int height;
		private readonly IBiome biome;

		public SimpleHeightmapColumnGenerator(IBiome biome, int height) {
			this.height = height;
			this.biome = biome;
		}

		public float SampleDensity(int x, int y) {
			return y - height;
		}

		public BlockState ResolveBlock(int x, int y) {
			return biome.ResolveBlock(SampleDensity(x, y), x, y);
		}
	}
}
