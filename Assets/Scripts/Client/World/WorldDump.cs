using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Client.World.Entity;
using SoulboundBackend.Client.World.Structure;
using System;
using System.Collections.Generic;

#nullable enable

namespace SoulboundBackend.Client.World {
	public struct WorldDump {
		public WorldChunk[]? generatedChunks;
		public Dictionary<int, List<StructurePlacement>> structurePlacements;
		public SerializedEntity player;
		public Dictionary<Guid, SerializedEntity> serializedEntities;

		public WorldDump(WorldChunk[] generatedChunks, SerializedEntity player, Dictionary<int, List<StructurePlacement>> structurePlacements,
						 Dictionary<Guid, SerializedEntity> serializedEntities) {
			this.generatedChunks = generatedChunks;
			this.structurePlacements = structurePlacements;
			this.serializedEntities = serializedEntities;
			this.player = player;
		}
	}
}
