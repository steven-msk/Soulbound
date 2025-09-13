using SoulboundBackend.Client.World;
using UnityEngine;

namespace SoulboundBackend.Client.Debug {
	public class BlockCoordinateVisual : DebugVisualUpdater, IDebugVisual<BlockPos> {
		[SerializeField] private string format;

		public string FormatValue(BlockPos value) => string.Format(format, value.x, value.y);
	}
}