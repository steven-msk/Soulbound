using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public readonly struct PreliminaryStructureData {
    public readonly BoundsInt2D estimatedBounds;
    public readonly ChunkBlockPos origin;
    // orientation, rotation?
    // randomness seed?

    public PreliminaryStructureData(BoundsInt2D estimatedBounds, ChunkBlockPos origin) {
        this.estimatedBounds = estimatedBounds;
        this.origin = origin;
    }
}
