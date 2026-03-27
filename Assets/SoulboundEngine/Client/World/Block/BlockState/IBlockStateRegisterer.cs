using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.World.BlockSystem.States {
	public interface IBlockStateRegisterer {
		BlockState AddWithProperties(BlockPropertyEntries properties);
		BlockState Add(BlockState state);
		void FinishRegistry();
		void SetBlock(Block block);
	}
}
