using SoulboundBackend.Client.World.Chunk;
using UnityEngine;

namespace SoulboundBackend.Client.Debug {
    public class ChunkCoordinateVisual : DebugVisualUpdater, IDebugVisual<ChunkBlockPos> {
        [SerializeField] private string format;

        public string FormatValue(ChunkBlockPos value) => string.Format(format, value.x, value.y, value.chunkX);
    }
}
