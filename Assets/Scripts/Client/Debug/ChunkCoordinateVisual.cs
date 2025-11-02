using SoulboundBackend.Client.World.Chunk;
using UnityEngine;

namespace SoulboundBackend.Client.Debug {
    public class ChunkCoordinateVisual : DebugVisualUpdater, IDebugVisual<ChunkBlockPos> {
        public string FormatValue(ChunkBlockPos value) => value.ToString();
    }
}
