using UnityEngine;

public class ChunkCoordinateVisual : DebugVisualUpdater, IDebugVisual<ChunkBlockPos> {
    [SerializeField] private string format;

    public string FormatValue(ChunkBlockPos value) => string.Format(format, value.x, value.y, value.chunkX);
}
