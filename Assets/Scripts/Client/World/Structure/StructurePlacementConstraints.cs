using System.Collections.Generic;

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
