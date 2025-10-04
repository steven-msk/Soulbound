using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Common;
using System.Collections.Generic;

namespace SoulboundBackend.Client.World.Structure {
    public readonly struct StructurePlacementConstraints {
        public readonly ChunkBlockPos origin;
        public readonly BoundsInt2D bounds;
        public readonly Dictionary<ChunkBlockPos, BlockState> stateOverrides;

        public StructurePlacementConstraints(ChunkBlockPos origin, BoundsInt2D bounds, Dictionary<ChunkBlockPos, BlockState> stateOverrides) {
            this.origin = origin;
            this.bounds = bounds;
            this.stateOverrides = stateOverrides;
        }
    }
}
