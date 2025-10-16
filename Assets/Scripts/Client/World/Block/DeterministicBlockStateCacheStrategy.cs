using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Plastic.Antlr3.Runtime;

namespace SoulboundBackend.Client.World.BlockSystem {
    public class DeterministicBlockStateCacheStrategy : IPersistentBlockStateCacheStrategy, IBlockStateCacheResettable {
        private readonly Dictionary<int, BlockState> cache = new();

        public void Initialize(Block block) {
            if (block.GetPredefinedStates(out var predefined)) {
                foreach (var state in predefined) {
                    cache[state.hash] = state;
                }
            }
        }

        public void RegisterDefault(BlockState defaultState) {
            cache[defaultState.hash] = defaultState;
        }

        public BlockState Get(Block block, BlockStateProperties properties) {
            int hash = properties.GetHashCode();
            if (!cache.TryGetValue(hash, out var state)) {
                state = BlockState.From(block, properties);
                cache[hash] = state;
            }
            return state;
        }

        public BlockState Get(Block block, int hash) {
            if (!cache.TryGetValue(hash, out var state)) {
                state = StateFileHandler.LoadSingle(block, hash);
                cache[hash] = state;
            }
            return state;
        }

        public void Save(Block block) {
            StateFileHandler.Save(block, cache.Values);
        }

        public void ResetCache() => cache.Clear();
    }
}
