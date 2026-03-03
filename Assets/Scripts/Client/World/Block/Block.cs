using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Common;

using SoulboundBackend.Core.AssetManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable

namespace SoulboundBackend.Client.World.BlockSystem {
	public abstract partial class Block {
		public string id { get; private set; } = null!;
		public abstract string name { get; init; }
		[Obsolete] public abstract BlockItem? itemReference { get; init; }
		public virtual BreakRequirement? breakRequirement { get; init; } = null;

		public abstract AssetKey tileKey { get; init; }

		private readonly BlockPropertyPool propertyPool = new();

		public BlockState defaultState { get; private set; } = null!;
		public virtual bool hasTileEntity { get; protected set; } = false;

		protected Block(string id) => this.ConstructNonGeneric(id);

		protected Block(string id, string name, AssetKey tileKey, BlockItem itemReference, BreakRequirement? breakRequirement) {
			this.name = name;
			this.tileKey = tileKey;
			this.itemReference = itemReference;
			this.breakRequirement = breakRequirement;
			this.ConstructNonGeneric(id);
		}

		private void ConstructNonGeneric(string id) {
			this.id = id;
			this.hashedID = HashHelper.StableHash(id);

			RegisterProperties(propertyPool);
			BlockPropertyEntries propertyEntries = propertyPool.CreateEntries();
			BlockStateRegisterer stateRegisterer = new(this);

			defaultState = CreateDefaultState(stateRegisterer, propertyEntries);
			CreateStates(stateRegisterer, propertyEntries);

			stateRegisterer.Add(defaultState);
			stateRegisterer.PostAll();
		}

		protected virtual void RegisterProperties(BlockPropertyPool pool) {
		}

		protected virtual BlockState CreateDefaultState(BlockStateRegisterer registerer, BlockPropertyEntries propertyEntries) {
			return registerer.AddWithProperties(propertyEntries);
		}

		protected virtual void CreateStates(BlockStateRegisterer registerer, BlockPropertyEntries properties) {
		}

		public virtual TileEntity? GetTileEntity(WorldChunk chunk, BlockPos blockPos) {
			return null;
		}

		public virtual BlockState Place(ItemStack itemStack, BlockPos blockPos) {
			return defaultState;
		}
		public virtual void OnNeighborStateChanged(BlockPos selfPos, BlockPos neighborPos, BlockState? oldState, BlockState? newState) {
		}
		public abstract IEnumerable<ItemStack> GetDrops(BlockState blockState, BreakSource source);
	}
}
