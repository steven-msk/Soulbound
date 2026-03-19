using SoulboundBackend.Client.World.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundBackend.Client.World.Generation {
	public struct ChunkBiomePartition {
		public IBiome primary;
		public IBiome? secondary;
		public int splitX;

		public bool hasBorder => secondary != null;
	}
}
