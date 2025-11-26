using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.Tilemaps;

#nullable enable

namespace SoulboundBackend.Client.World.BlockSystem {
	public abstract partial class Block {
		private static readonly Logger logger = Logger.CreateInstance();
		public abstract string name { get; }
		public abstract TileBase tileReference { get; }
		public abstract BlockItem? itemReference { get; }
		public virtual BreakRequirement? breakRequirement => null;

		protected Dictionary<IBlockStateProperty, object> propertyMap = new();
		public IList<IBlockStateProperty> propertyDefinitions => propertyMap.Keys.AsReadOnlyList();
		public bool propertyDefinitionTerminated { get; protected set; } = false;
		public IBlockStateCacheStrategy stateCacheStrategy { get; protected set; } = new StaticStateCache();

		public BlockState defaultState { get; private set; }

		protected Block(IBlockStateCacheStrategy stateCacheStrategy) {
			this.stateCacheStrategy = stateCacheStrategy;

			RegisterProperties();
			propertyDefinitionTerminated = true;
			RegisterDefaultState(CreateDefaultState());
			stateCacheStrategy.Initialize(this);
		}

		protected abstract void RegisterProperties();
		protected abstract BlockState CreateDefaultState();
		public virtual bool GetPredefinedStates(out IReadOnlyList<BlockState> states) {
			states = new List<BlockState>();
			return false;
		}
		public virtual BlockState Place(ItemStack itemStack, BlockPos blockPos) {
			return defaultState;
		}

		public void RegisterProperty<T>(BlockProperty<T> property, T defaultValue) {
			if (propertyDefinitionTerminated) {
				logger.LogWarning("Cancelled block property registration '{}' due to definition context termination", property.name);
				return;
			}
			propertyMap.Add(property, defaultValue!);
		}

		public object GetDefaultValueOfProperty(IBlockStateProperty property) {
			if (!this.HasProperty(property)) {
				return null!;
			}
			return propertyMap[property];
		}

		protected void RegisterDefaultState(BlockState state) {
			defaultState = state;
			stateCacheStrategy.RegisterDefault(state);
		}

		public BlockState GetStateFor(BlockStateProperties properties) {
			return stateCacheStrategy.Get(this, properties);
		}

		public bool TryGetStateByHash(int hash, out BlockState state) {
			state = stateCacheStrategy.Get(this, hash);
			return state is not null;
		}

		public bool HasProperty(IBlockStateProperty property) {
			return propertyMap.ContainsKey(property);
		}

		public BlockState WithProperty<T>(BlockState state, BlockProperty<T> property, T value) {
			return state.With(property, value);
		}

		public virtual IBlockStateBehavior CreateBehaviorFor(BlockStateProperties properties) {
			return defaultState.stateBehavior;
		}

		internal int ComputeHash(object obj) => obj.GetHashCode();

		public override string ToString() {
			return name;
		}
	}
}