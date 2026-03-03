using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.BlockSystem {
	public static class BlockStateRegistry {
		static readonly List<BlockState> states = new();
		static readonly Dictionary<int, int> hashToID = new();

		public static int Register(BlockState state) {
			int hash = state.stateHash;

			if (hashToID.TryGetValue(hash, out int existing)) {
				return existing;
			}
			int id = states.Count;
			states.Add(state);
			state.stateID = id;
			hashToID[hash] = id;
			return id;
		}

		public static BlockState Get(int stateID) => states[stateID];

		public static bool TryGetByHash(int hash, out BlockState state) {
			if (hashToID.TryGetValue(hash, out int stateID)) {
				state = states[stateID];
				return true;
			}
			state = default;
			return false;
		}
	}
}
