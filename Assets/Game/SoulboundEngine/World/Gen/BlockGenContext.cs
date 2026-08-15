using SoulboundEngine.World.Block;
using UnityEngine;

namespace SoulboundEngine.World.Gen {
	public struct BlockGenContext {
		public BlockPos pos;
		public int surfaceY;
		public float caveDensity;
		public bool isCave;

		public int distanceToSurface => Mathf.Abs(this.surfaceY - this.pos.y);
		public int signedDistanceToSurface => this.surfaceY - this.pos.y;

		public bool AboveSurface() {
			return this.pos.y > this.surfaceY;
		}
	}
}
