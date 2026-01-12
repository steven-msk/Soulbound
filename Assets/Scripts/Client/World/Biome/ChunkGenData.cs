using Assets.Scripts.Client.World.Biome;
using SoulboundBackend.Client.World.Chunk;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundBackend.Client.World.Generation {
	public struct ChunkGenData {
		public WorldChunk chunk;
		public BlockGenContext[][] genContexts;
		public int[] surfacePoints;
		public IEnumerable<BiomeWeight>[] biomeWeights;
		public ChunkBiomePartition biomePartition;
		public float[][] caveDensities;
		public BitArray[] caveMask;
	}
}
