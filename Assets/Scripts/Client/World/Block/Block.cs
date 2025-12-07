using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Common;
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
		public string id { get; private set; }
		public abstract string name { get; init; }
		public abstract TileBase tileReference { get; init; }
		public abstract BlockItem? itemReference { get; init; }
		public virtual BreakRequirement? breakRequirement { get; init; } = null;

		private Dictionary<int, BlockState> statesByHash = new();
		private readonly BlockPropertyPool propertyPool = new();

		public BlockState defaultState { get; private set; }
		public virtual bool hasTileEntity { get; protected set; } = false;

		protected Block(string id) => this.ConstructNonGeneric(id);

		protected Block(string id, string name, TileBase tileReference, BlockItem itemReference, BreakRequirement? breakRequirement) {
			this.name = name;
			this.tileReference = tileReference;
			this.itemReference = itemReference;
			this.breakRequirement = breakRequirement;
			this.ConstructNonGeneric(id);
		}

		private void ConstructNonGeneric(string id) {
			this.id = id;
			this.hashedID = HashHelper.StableHash(id);

			RegisterProperties(propertyPool);
			defaultState = CreateDefaultState(propertyPool);

			var stateRegisterer = new BlockStateRegisterer(this);
			CreateStates(stateRegisterer);

			stateRegisterer.Register(defaultState);
			statesByHash = stateRegisterer.PostAll();

		}

		protected abstract void RegisterProperties(BlockPropertyPool pool);
		protected abstract BlockState CreateDefaultState(BlockPropertyPool propertyPool);
		public virtual void CreateStates(BlockStateRegisterer registerer) {
		}
		public virtual TileEntity GetTileEntity(WorldChunk chunk, BlockPos blockPos) {
			return null!;
		}

		public virtual BlockState Place(ItemStack itemStack, BlockPos blockPos) {
			return defaultState;
		}
		public virtual void OnNeighborStateChanged(BlockPos selfPos, BlockPos neighborPos, BlockState oldState, BlockState newState) {
		}
		public abstract IEnumerable<ItemStack> GetDrops(BlockState blockState, BreakSource source);

		public bool TryGetState(int hash, out BlockState state) {
			return statesByHash.TryGetValue(hash, out state);
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