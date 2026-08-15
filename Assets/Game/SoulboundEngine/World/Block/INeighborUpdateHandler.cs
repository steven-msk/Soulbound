using SoulboundEngine.Client.World.Level;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.World.Block {
	public interface INeighborUpdateHandler {
		void OnNeighborChanged(Level.Level level, BlockPos selfPos, BlockPos neighborPos);
	}
}
