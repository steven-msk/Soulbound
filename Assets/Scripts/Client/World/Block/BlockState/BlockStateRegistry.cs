using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.BlockSystem {
	public static class BlockStateRegistry {
		static readonly Dictionary<int, BlockState> stateByHash = new();

		public static int Register(BlockState state) {
			int hash = state.stateHash;
			UnityEngine.Debug.Log("registering hash " + hash  + " block " + state.block);
			if (stateByHash.ContainsKey(hash)) {
				return hash;
			}
			stateByHash[hash] = state;
			return hash;
		}

		public static BlockState Get(int hash) {
			return stateByHash[hash];
		}

		public static bool TryGet(int hash, out BlockState state) {
			return stateByHash.TryGetValue(hash, out state);
		}

		public static IEnumerable<BlockState> All() {
			return stateByHash.Values.AsEnumerable();
		}
	}
}
