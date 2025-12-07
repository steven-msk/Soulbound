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

		private Dictionary<int, BlockState> statesByHash = new();
		private readonly BlockPropertyPool propertyPool = new();

		public BlockState defaultState { get; private set; }

		protected Block() {
			RegisterProperties(propertyPool);
			defaultState = CreateDefaultState(propertyPool);

			var stateRegisterer = new BlockStateRegisterer(this);
			CreateStates(stateRegisterer);

			stateRegisterer.Register(defaultState);
			statesByHash = stateRegisterer.PostAll();
		}

		protected abstract void RegisterProperties(BlockPropertyPool pool);
		protected abstract BlockState CreateDefaultState(BlockPropertyPool propertyPool);
		[Obsolete]
		public virtual bool GetPredefinedStates(out IReadOnlyList<BlockState> states) {
			states = new List<BlockState>();
			return false;
		}
		public virtual void CreateStates(BlockStateRegisterer registerer) {
		}

		public virtual BlockState Place(ItemStack itemStack, BlockPos blockPos) {
			return defaultState;
		}
		public virtual void OnNeighborStateChanged(BlockPos selfPos, BlockPos neighborPos, BlockState oldState, BlockState newState) {
		}
		public abstract IEnumerable<ItemStack> GetDrops(BlockState blockState, BreakSource source);

		[Obsolete]
		protected void RegisterDefaultState(BlockState state) {
			//defaultState = state;
			//stateCacheStrategy.RegisterDefault(state);
		}

		[Obsolete]
		public bool TryGetStateByHash(int hash, out BlockState state) {
			//state = stateCacheStrategy.Get(this, hash);
			//return state is not null;
			state = null;
			return false;
		}

		public bool HasProperty(IBlockStateProperty property) {
			return propertyPool.Has(property.name);
		}

		public override string ToString() {
			return $"Block[" +
				$"name:{name}, " +
				$"tileReference:{tileReference}, " +
				$"itemReference:{itemReference}, " +
				$"propertyPool:{propertyPool}]";
		}
	}
}