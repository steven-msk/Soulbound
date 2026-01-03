using Assets.Scripts.Client.World.Biome;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.Chunk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundBackend.Client.World.Generation {
	public class Heightmap {
		public int ypos { get; private set; }
		public int height => WorldChunk.maxY - ypos;

		public Heightmap(int ypos) {
			this.ypos = ypos;
		}
	}
}
