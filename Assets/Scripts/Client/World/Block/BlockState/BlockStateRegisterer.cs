using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.BlockSystem {
	public sealed class BlockStateRegisterer {
		private readonly List<BlockState> registered = new();
		private readonly Block block;

		public BlockStateRegisterer(Block block) {
			this.block = block;
		}

		public BlockState AddWithProperties(BlockPropertyEntries properties) {
			BlockState state = new(block, properties);
			return Add(state);
		}

		public BlockState Add(BlockState state) {
			if (!registered.Contains(state)) registered.Add(state);
			return state;
		}

		public void PostAll() {
			foreach (var blockState in registered) {
				BlockStateRegistry.Register(blockState);
			}
		}

	}
}
