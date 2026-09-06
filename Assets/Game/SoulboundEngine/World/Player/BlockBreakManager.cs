namespace SoulboundEngine.World.Player {
	using SoulboundEngine.Common.Math;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Block.State;

#nullable enable

	public sealed class BlockBreakManager {
		private float accumulated;
		private BlockPos? blockPos;
		private BlockState? target;

		public bool Reset(BlockState target, BlockPos blockPos) {
			if (this.blockPos == blockPos) return false;

			this.accumulated = 0f;
			this.target = target;
			this.blockPos = blockPos;
			return true;
		}

		public void Reset() {
			this.accumulated = 0f;
			this.blockPos = null;
			this.target = null;
		}

		public bool Tick(float speed) {
			if (this.target == null) return false;
			this.accumulated += speed;
			return this.accumulated >= this.target.GetHardness();
		}

		public float GetProgress() {
			return this.target == null ? 0f : Maths.Clamp01(this.accumulated / this.target.GetHardness());
		}

		public BlockPos? GetBlockPos() => this.blockPos; 
	}
}
