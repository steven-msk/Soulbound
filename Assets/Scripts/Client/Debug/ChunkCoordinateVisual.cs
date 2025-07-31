using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ChunkCoordinateVisual : DebugVisualUpdater, IDebugVisual<ChunkBlockPos> {
    [SerializeField] private string format;

    public string FormatValue(ChunkBlockPos value) => string.Format(format, value.x, value.y, value.chunkX);
}
