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


		public BlockState defaultState { get; private set; } = null!;
		public virtual bool hasTileEntity { get; protected set; } = false;

		protected Block(string id) => this.ConstructNonGeneric(id);

		protected Block(string id, string name, BlockItem itemReference, BreakRequirement? breakRequirement) {
			this.name = name;
			this.itemReference = itemReference;
			this.breakRequirement = breakRequirement;
			this.ConstructNonGeneric(id);
		}

		private void ConstructNonGeneric(string id) {
			this.id = id;
			this.hashedID = HashHelper.StableHash(id);

			BlockPropertyEntries properties = new();
			BlockStateRegisterer stateRegisterer = new(this);

			CreateStates(stateRegisterer, properties);
			defaultState = GetDefaultState(stateRegisterer, properties);
			stateRegisterer.Add(defaultState);

			stateRegisterer.PostAll();
			BlockRegistry.Register(this);
		}

		public abstract AssetKey GetRenderTileKey(BlockState blockState);

		protected virtual BlockState GetDefaultState(BlockStateRegisterer registerer, BlockPropertyEntries properties) {
			return registerer.AddWithProperties(properties);
		}

		protected virtual void CreateStates(BlockStateRegisterer registerer, BlockPropertyEntries properties) {
		}

		public virtual bool HasTileEntity(Level level, BlockPos blockPos, BlockState blockState) => false;
		public virtual TileEntity? GetTileEntity(Level level, BlockPos blockPos) {
			return null;
		}

		public virtual BlockState Place(ItemStack itemStack, BlockPos blockPos) {
			return defaultState;
		}
		public virtual void OnNeighborStateChanged(BlockPos selfPos, BlockPos neighborPos, BlockState? oldState, BlockState? newState) {
		}
		public virtual IEnumerable<ItemStack> GetDrops(BlockState blockState, BreakSource source) {
			yield break;
		}
	}
}
