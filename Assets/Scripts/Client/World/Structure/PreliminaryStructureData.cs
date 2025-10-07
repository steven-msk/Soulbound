using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Common;
using UnityEngine;

namespace SoulboundBackend.Client.World.Structure {
    public readonly struct PreliminaryStructureData {
        public readonly Vector2Int size;
        public readonly ChunkBlockPos origin;
        public readonly bool forced;
        // orientation, rotation?
        // randomness seed?

        public PreliminaryStructureData(Vector2Int size, ChunkBlockPos origin, bool forced) {
            this.size = size;
            this.origin = origin;
            this.forced = forced;
        }
    }
}
