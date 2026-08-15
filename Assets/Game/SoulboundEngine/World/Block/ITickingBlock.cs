using SoulboundEngine.Client.World.Block.State;
using SoulboundEngine.Client.World.Level;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.World.Block {
	public interface ITickingBlock {
		void Tick(Level.Level level, BlockPos blockPos, BlockState blockState);
	}
}
