using SoulboundBackend.Client.World.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.Generation {
	public struct BiomeWeight {
		public IBiome biome;
		public float value;

		public BiomeWeight(IBiome biome, float value) {
			this.biome = biome;
			this.value = value;
		}
	}
}
