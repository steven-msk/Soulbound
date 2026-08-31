namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.World.Block;
	using System;

	public struct BlockGenContext {
		public BlockPos pos;
		public int surfaceY;
		public float caveDensity;
		public bool isCave;

		public readonly int distanceToSurface => Math.Abs(this.surfaceY - this.pos.y);
		public readonly int signedDistanceToSurface => this.surfaceY - this.pos.y;

		public readonly bool AboveSurface() {
			return this.pos.y > this.surfaceY;
		}
	}
}
