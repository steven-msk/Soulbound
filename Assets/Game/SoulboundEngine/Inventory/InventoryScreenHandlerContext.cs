namespace SoulboundEngine.Inventory {
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Level;
	using System;

	public sealed class InventoryScreenHandlerContext {
		public static readonly InventoryScreenHandlerContext EMPTY = new(default, null, true);
		private readonly BlockPos blockPos;
		private readonly Level level;
		private readonly bool empty;

		private InventoryScreenHandlerContext(BlockPos blockPos, Level level, bool empty) {
			this.blockPos = blockPos;
			this.level = level;
			this.empty = empty;
		}

		public static InventoryScreenHandlerContext Of(BlockPos blockPos, Level level) {
			return new InventoryScreenHandlerContext(blockPos, level, false);
		}

		public void Run(Action<BlockPos, Level> action) {
			if (!this.empty) {
				action(this.blockPos, this.level);
			}
		}
	}
}
