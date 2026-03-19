using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Client.World.EntitySystem;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

#nullable enable

namespace SoulboundBackend.Client.World.Serialization {
	public struct WorldDump {
		public int seed;
		public WorldChunk[]? generatedChunks;
		public bool nonNulled;

		[JsonConstructor]
		public WorldDump(
				int seed, 
				WorldChunk[] generatedChunks
			) {
			this.seed = seed;
			this.generatedChunks = generatedChunks;
			this.nonNulled = true;
		}
	}
}
