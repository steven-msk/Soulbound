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

		public BlockState Register(BlockPropertyEntries properties) {
			var state = new BlockState(block, properties);
			registered.Add(state);
			return state;
		}

		public BlockState Register(BlockState state) {
			registered.Add(state);
			return state;
		}

		public Dictionary<int, BlockState> PostAll() {
			return registered.ToDictionary(s => BlockStateRegistry.Register(s), s => s);
		}
	}
}
