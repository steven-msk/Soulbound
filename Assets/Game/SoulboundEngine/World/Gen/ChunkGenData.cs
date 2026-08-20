
using SoulboundEngine.World.Biome;
using SoulboundEngine.World.Chunk;
using System;
using System.Collections;
using System.Collections.Generic;

#nullable enable

namespace SoulboundEngine.World.Gen {
	public struct ChunkGenData {
		[Obsolete] public WorldChunk chunk;
		public BlockGenContext[][] genContexts;
		public int[] surfacePoints;
		public IEnumerable<BiomeWeight>[] biomeWeights;
		public ChunkBiomePartition biomePartition;
		public float[][] caveDensities;
		public BitArray[] caveMask;
	}
}
