public readonly struct PreliminaryStructureData {
    public readonly BoundsInt2D estimatedBounds;
    public readonly ChunkBlockPos origin;
    public readonly bool forced;
    // orientation, rotation?
    // randomness seed?

    public PreliminaryStructureData(BoundsInt2D estimatedBounds, ChunkBlockPos origin, bool forced) {
        this.estimatedBounds = estimatedBounds;
        this.origin = origin;
        this.forced = forced;
    }
}
