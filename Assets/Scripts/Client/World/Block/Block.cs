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

		[Obsolete]
		protected Dictionary<IBlockStateProperty, object> propertyMap = new();
		[Obsolete]
		public IList<IBlockStateProperty> propertyDefinitions => propertyMap.Keys.AsReadOnlyList();
		[Obsolete]
		public bool propertyDefinitionTerminated { get; protected set; } = false;
		public IBlockStateCacheStrategy stateCacheStrategy { get; protected set; } = new StaticStateCache();
		private readonly BlockPropertyPool propertyPool = new();

		public BlockState defaultState { get; private set; }

		protected Block(IBlockStateCacheStrategy stateCacheStrategy) {
			this.stateCacheStrategy = stateCacheStrategy;

			RegisterProperties(propertyPool);
			propertyDefinitionTerminated = true;
			RegisterDefaultState(CreateDefaultState(propertyPool));
			stateCacheStrategy.Initialize(this);
		}

		protected abstract void RegisterProperties(BlockPropertyPool pool);
		protected abstract BlockState CreateDefaultState(BlockPropertyPool propertyPool);
		public virtual bool GetPredefinedStates(out IReadOnlyList<BlockState> states) {
			states = new List<BlockState>();
			return false;
		}

		public virtual BlockState Place(ItemStack itemStack, BlockPos blockPos) {
			return defaultState;
		}
		public virtual void OnNeighborStateChanged(BlockPos selfPos, BlockPos neighborPos, BlockState oldState, BlockState newState) {
		}
		public abstract IEnumerable<ItemStack> GetDrops(BlockState blockState, BreakSource source);

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
			return propertyPool.Has(property.name);
			//return propertyMap.ContainsKey(property);
		}

		[Obsolete]
		public BlockState WithProperty<T>(BlockState state, BlockProperty<T> property, T value) {
			return state.With(property, value);
		}

		//[Obsolete]
		//public virtual IBlockStateBehavior CreateBehaviorFor(BlockStateProperties properties) {
		//	return defaultState.stateBehavior;
		//}

		internal int ComputeHash(object obj) => obj.GetHashCode();

		public override string ToString() {
			return name;
		}
	}
}