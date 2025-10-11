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
		protected Dictionary<int, BlockState> cachedStates = new();

		protected Block() {
			RegisterProperties();
			RegisterDefaultState(CreateDefaultState());
			InitializePredefinedStates();
		}

		protected abstract void RegisterProperties();
		protected abstract BlockState CreateDefaultState();
		protected virtual void InitializePredefinedStates() {
		}

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

		public BlockState WithProperty<T>(BlockState state, BlockProperty<T> property, T value) {
			return state.With(property, value);
		}

		protected virtual IBlockStateBehavior CreateBehaviorFor(Dictionary<string, object> properties) {
			return defaultState.stateBehavior;
		}

		private int ComputeHash(object obj) => obj.GetHashCode();

		public override string ToString() {
			return name;
		}
	}
}