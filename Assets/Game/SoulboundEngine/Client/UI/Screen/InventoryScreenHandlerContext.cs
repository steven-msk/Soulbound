using SoulboundEngine.Client.World.Block;
using SoulboundEngine.Client.World.Level;
using System;

namespace SoulboundEngine.Client.UI.Screen {
	public sealed class InventoryScreenHandlerContext {
		public static readonly InventoryScreenHandlerContext EMPTY = new(null, default, null, true);
		private readonly SoulboundClient client;
		private readonly BlockPos blockPos;
		private readonly Level level;
		private readonly bool empty;

		private InventoryScreenHandlerContext(SoulboundClient client, BlockPos blockPos, Level level, bool empty) {
			this.client = client;
			this.blockPos = blockPos;
			this.level = level;
			this.empty = empty;
		}

		public static InventoryScreenHandlerContext Of(SoulboundClient client, BlockPos blockPos, Level level) {
			return new InventoryScreenHandlerContext(client, blockPos, level, false);
		}

		public void Run(Action<SoulboundClient, BlockPos, Level> action) {
			if (!this.empty) {
				action(this.client, this.blockPos, this.level);
			}
		}
	}
}
