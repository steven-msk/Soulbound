using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.Generation {
	public struct TerrainModulation {
		public float heightOffset;
		public float amplitude;
		public float erosion;

		public TerrainModulation(float heightOffset, float amplitude, float erosion) {
			this.heightOffset = heightOffset;
			this.amplitude = amplitude;
			this.erosion = erosion;
		}
	}
}
