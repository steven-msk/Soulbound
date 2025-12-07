using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World  {
	public sealed class TileEntityTickManager : TickManager {
		public void AddTileEntity(TileEntity tileEntity) {
			if (tileEntity is ITickable tickable) {
				this.AddTickable(tickable);
			}
		}

		public void RemoveTileEntity(TileEntity tileEntity) {
			if (tileEntity is ITickable tickable) {
				this.RemoveTickable(tickable);
			}
		}
	}
}
