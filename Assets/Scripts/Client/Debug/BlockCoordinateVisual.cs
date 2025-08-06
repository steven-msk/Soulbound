using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BlockCoordinateVisual : DebugVisualUpdater, IDebugVisual<BlockPos> {
    [SerializeField] private string format;

    public string FormatValue(BlockPos value) => string.Format(format, value.x, value.y);
}