using SoulboundBackend.Client.ItemSystem;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

#nullable enable

namespace SoulboundBackend.Client.World.BlockSystem {
	public abstract partial class Block {
		public abstract string name { get; }
		public abstract TileBase tileReference { get; }
		public abstract BlockItem? itemReference { get; }

		protected List<object> propertyList = new();
		public BlockState defaultState { get; private set; }
		private Dictionary<int, BlockState> cachedStates = new();

		protected void RegisterDefaultState(BlockState state) {
			defaultState = state;
			cachedStates[ComputeHash(state)] = state;
		}

		public BlockState GetStateFor(Dictionary<string, object> properties) {
			int hash = ComputeHash(properties);

			if (!cachedStates.TryGetValue(hash, out var state)) {
				state = new BlockState(this, properties, CreateBehaviorFor(properties));
				cachedStates[hash] = state;
			}
			return state;
		}

		public bool HasProperty<T>(BlockProperty<T> property) {
			return propertyList.Contains(property);
		}

		protected virtual IBlockStateBehavior CreateBehaviorFor(Dictionary<string, object> properties) {
			return defaultState.stateBehavior;
		}

		public abstract void RegisterProperties();

		private int ComputeHash(object obj) => obj.GetHashCode();

		public override string ToString() {
			return name;
		}
	}
}