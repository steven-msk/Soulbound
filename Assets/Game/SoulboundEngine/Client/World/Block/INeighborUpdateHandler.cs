using SoulboundEngine.Client.World.LevelDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.World.BlockSystem {
	public interface INeighborUpdateHandler {
		void OnNeighborChanged(Level level, BlockPos selfPos, BlockPos neighborPos);
	}
}
