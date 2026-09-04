namespace SoulboundEngine.World.Player {
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Block.State;

	public sealed class BlockBreakManager {
		private float accumulated;
		private BlockPos blockPos;
		private BlockState target;

		public bool Reset(BlockState target, BlockPos blockPos) {
			if (this.blockPos == blockPos) return false;

			this.accumulated = 0f;
			this.target = target;
			this.blockPos = blockPos;
			return true;
		}

		public bool Tick(float speed) {
			this.accumulated += speed;
			return this.accumulated >= this.target.GetHardness();
		}
	}
}
