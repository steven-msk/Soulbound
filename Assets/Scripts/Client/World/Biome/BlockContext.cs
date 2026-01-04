using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.Generation {
	public struct BlockContext {
		public BlockPos pos;
		public float surfaceY;
		public float distanceToSurface => surfaceY - pos.y;

		public bool AboveSurface() {
			return pos.y > surfaceY;
		}
	}
}
