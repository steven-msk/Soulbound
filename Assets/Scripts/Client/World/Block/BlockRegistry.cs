using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.BlockSystem {
	public static class BlockRegistry {
		static readonly Dictionary<string, Block> idToBlock = new();

		public static void Register(Block block) {
			if (!idToBlock.ContainsKey(block.id)) {
				idToBlock[block.id] = block;
			}
		}

		public static bool TryGet(string id, out Block block) {
			return idToBlock.TryGetValue(id, out block);
		}

		public static IEnumerable<Block> AllBlocks() => idToBlock.Values;
	}
}
