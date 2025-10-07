using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Common;
using System.Collections.Generic;

namespace SoulboundBackend.Client.World.Structure {
    public readonly struct StructurePlacementConstraints {
        public readonly BoundsInt2D bounds;
        public readonly Dictionary<ChunkBlockPos, BlockState> stateOverrides;

        public StructurePlacementConstraints(BoundsInt2D bounds, Dictionary<ChunkBlockPos, BlockState> stateOverrides) {
            this.bounds = bounds;
            this.stateOverrides = stateOverrides;
        }
    }
}
