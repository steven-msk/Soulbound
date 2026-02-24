using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Client.World.EntitySystem;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

#nullable enable

namespace SoulboundBackend.Client.World {
	public struct WorldDump {
		public int seed;
		public WorldChunk[]? generatedChunks;
		//public Dictionary<int, List<StructurePlacement>> structurePlacements;
		public SerializedEntity player;
		public Dictionary<Guid, SerializedEntity> serializedEntities;
		public bool nonNulled;

		[JsonConstructor]
		public WorldDump(
				int seed, 
				WorldChunk[] generatedChunks,
				SerializedEntity player, 
				//Dictionary<int, List<StructurePlacement>> structurePlacements,
				Dictionary<Guid, SerializedEntity> serializedEntities
			) {
			this.seed = seed;
			this.generatedChunks = generatedChunks;
			//this.structurePlacements = structurePlacements;
			this.serializedEntities = serializedEntities;
			this.player = player;
			this.nonNulled = true;
		}
	}
}
