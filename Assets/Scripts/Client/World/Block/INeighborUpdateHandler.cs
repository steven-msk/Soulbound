using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.BlockSystem {
	public interface INeighborUpdateHandler {
		void OnNeighborChanged(Level level, BlockPos selfPos, BlockPos neighborPos);
	}
}
