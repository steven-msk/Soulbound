using SoulboundEngine.Client.World.Biome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundEngine.Client.World.Biome {
	public struct ChunkBiomePartition {
		public IBiome primary;
		public IBiome? secondary;
		public int splitX;

		public bool hasBorder => secondary != null;
	}
}
