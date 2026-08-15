using System.Collections.Generic;

namespace SoulboundEngine.World.Block.Entity {
	public sealed class TileEntityTickManager {
		private readonly List<ITickable> tickables = new();

		public void Tick() {
			foreach (var tickable in this.tickables.ToArray()) {
				tickable.Tick();
			}
		}

		public void AddTileEntity(TileEntity tileEntity) {
			if (tileEntity is ITickable tickable) {
				this.tickables.Add(tickable);
			}
		}

		public void RemoveTileEntity(TileEntity tileEntity) {
			if (tileEntity is ITickable tickable) {
				this.tickables.Remove(tickable);
			}
		}
	}
}
