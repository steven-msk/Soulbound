using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.BlockSystem.States {
	public class GlobalBlockStateRegisterer : IBlockStateRegisterer {
		private readonly List<BlockState> registered = new();
		private Block block;

		public BlockState AddWithProperties(BlockPropertyEntries properties) {
			if (block == null) throw new NotSupportedException("Block not set");

			BlockState state = new(block, properties);
			return Add(state);
		}

		public BlockState Add(BlockState state) {
			if (!registered.Contains(state)) registered.Add(state);
			return state;
		}

		public void FinishRegistry() {
			foreach (var blockState in registered) {
				BlockStateRegistry.Register(blockState);
			}
		}

		void IBlockStateRegisterer.SetBlock(Block block) {
			this.block = block;
		}
	}
}
