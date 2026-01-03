using SoulboundBackend.Client.World.Chunk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.Generation {
	public sealed class BiomeMap {
		private readonly IEnumerable<IBiome> biomes;

		public BiomeMap(IEnumerable<IBiome> biomes) {
			this.biomes = biomes;
		}

		public IBiome ResolveBiome(int blockX) {
			IBiome targetBiome = null;
			float maxDensity = float.MinValue;

			foreach (var biome in biomes) {
				float density = biome.GetDensity(blockX);
				if (density > maxDensity) {
					maxDensity = density;
					targetBiome = biome;
				}
			}

			return targetBiome;
		}
	}
}
