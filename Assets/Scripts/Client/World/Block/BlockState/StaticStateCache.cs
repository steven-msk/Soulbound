using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundBackend.Client.World.BlockSystem {
	[Obsolete]
	public class StaticStateCache : IBlockStateCacheStrategy {
		private BlockState? defaultState;

		public BlockState Get(Block block, BlockStateProperties properties) {
			return Get(block, properties.GetHashCode());
		}

		public BlockState Get(Block block, int hash) {
			return defaultState ?? throw new InvalidOperationException("Default state not set");
		}

		public void RegisterDefault(BlockState defaultState) {
			this.defaultState = defaultState;
		}

		public void Initialize(Block block) {
		}
	}
}
