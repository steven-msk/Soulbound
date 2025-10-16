using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.BlockSystem {
    public class FileLinkedStateCache : IPersistentStateCache, IBlockStateCacheResettable {
        private readonly Dictionary<int, BlockState> cache = new(); 

        public BlockState Get(Block block, BlockStateProperties properties) {
            int hash = properties.GetHashCode();
            if (cache.TryGetValue(hash, out var state)) {
                return state;
            }

            state = StateFileHandler.LoadSingle(block, hash);
            if (state is null) {
                state = BlockState.From(block, properties);
            }

            cache[hash] = state;
            return state;  
        }

        public BlockState Get(Block block, int hash) {
            var persistencyInfo = StateFileHandler.GetPersistencyInfo(block, hash);
            if (cache.TryGetValue(persistencyInfo.hash, out var state)) {
                return state;
            }

            state = persistencyInfo.ToBlockState(block);
            cache[hash] = state;
            return state;
        }

        public void RegisterDefault(BlockState defaultState) {
            cache[defaultState.hash] = defaultState;
        }

        public void Save(Block block) {
            StateFileHandler.Save(block, cache.Values);
        }

        public void Initialize(Block block) {
        }

        public void ResetCache() => cache.Clear();
    }
}
