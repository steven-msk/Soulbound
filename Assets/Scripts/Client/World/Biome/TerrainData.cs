using SoulboundBackend.Client.World.Chunk;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.Generation {
	public struct TerrainData {
		public WorldChunk chunk;
		public Dictionary<int, int> surfacePoints;
		public Dictionary<int, IEnumerable<BiomeWeight>> biomeWeights;
		public float[][] caveDensities;
		public BitArray[] caveMask;

		public bool IsCave(int chunkX, int yIndex) {
			return caveMask[chunkX][yIndex];
		}
	}
}
