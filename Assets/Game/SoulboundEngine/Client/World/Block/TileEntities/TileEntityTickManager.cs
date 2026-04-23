using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.World.BlockSystem.TileEntities {
	public sealed class TileEntityTickManager {
		private readonly List<ITickable> tickables = new();

		public void Tick() {
			foreach (var tickable in tickables.ToArray()) {
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
