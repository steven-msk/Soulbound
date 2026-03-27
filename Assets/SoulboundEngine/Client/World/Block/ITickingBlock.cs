using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Client.World.LevelDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.World.BlockSystem {
	public interface ITickingBlock {
		void Tick(Level level, BlockPos blockPos, BlockState blockState);
	}
}
