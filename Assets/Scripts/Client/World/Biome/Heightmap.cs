using Assets.Scripts.Client.World.Biome;
using SoulboundBackend.Client.World.BlockSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundBackend.Client.World.Generation {
	public class Heightmap {
		private readonly int height;

		public Heightmap(int height) {
			this.height = height;
		}

		public float Sample(int x, int y) {
			return height - y;
		}
	}
}
