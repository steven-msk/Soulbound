using SoulboundBackend.Client.World;
using UnityEngine;

namespace SoulboundBackend.Client.Debug {
	public class BlockCoordinateVisual : DebugVisualUpdater, IDebugVisual<BlockPos> {
		public string FormatValue(BlockPos value) => value.ToString();
	}
}