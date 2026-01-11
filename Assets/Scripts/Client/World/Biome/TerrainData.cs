using Assets.Scripts.Client.World.Biome;
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
		public BlockGenContext[][] genContexts;
		public Dictionary<int, int> surfacePoints;
		public IEnumerable<BiomeWeight>[] biomeWeights;
		public ChunkBiomePartition biomePartition;
		public float[][] caveDensities;
		public BitArray[] caveMask;

		public bool IsCave(int chunkX, int yIndex) {
			return caveMask[chunkX][yIndex];
		}
	}
}
