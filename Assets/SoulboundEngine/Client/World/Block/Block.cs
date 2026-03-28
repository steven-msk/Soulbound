using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.World.BlockSystem.Render;
using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Client.World.BlockSystem.TileEntities;
using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Core;
using System;
using System.Collections.Generic;

#nullable enable

namespace SoulboundEngine.Client.World.BlockSystem {
	public abstract partial class Block {
		private readonly string id;
		public abstract string name { get; init; }
		public abstract int minBreakLevel { get; init; }
		public BlockState defaultState { get; private set; } = null!;

		protected Block(string id, IBlockStateRegisterer? stateRegisterer = null) {
			this.id = id;
			RegisterStates(stateRegisterer);
		}

		protected Block(string id, string name, int minBreakLevel, IBlockStateRegisterer? stateRegisterer = null) {
			this.id = id;
			this.name = name;
			this.minBreakLevel = minBreakLevel;
			RegisterStates(stateRegisterer);
		}

		private void RegisterStates(IBlockStateRegisterer? stateRegisterer) {
			BlockPropertyEntries properties = new();
			stateRegisterer ??= new GlobalBlockStateRegisterer();
			stateRegisterer.SetBlock(this);

			CreateStates(stateRegisterer, properties);
			defaultState = GetDefaultState(stateRegisterer, properties);
			stateRegisterer.Add(defaultState);

			stateRegisterer.FinishRegistry();
		}

		public string GetID() => id;

		public abstract BlockRenderData GetRenderData(BlockState blockState);

		protected virtual BlockState GetDefaultState(IBlockStateRegisterer registerer, BlockPropertyEntries properties) {
			return registerer.AddWithProperties(properties);
		}

		protected virtual void CreateStates(IBlockStateRegisterer registerer, BlockPropertyEntries properties) {
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

		public readonly struct RegistrationKey : IRegistrationKey<Block> {
			public readonly string blockID;

			public RegistrationKey(string blockID) {
				this.blockID = blockID;
			}

			public override bool Equals(object obj) {
				return obj is RegistrationKey blockKey
					&& this.blockID == blockKey.blockID;
			}

			public override int GetHashCode() => HashCode.Combine(blockID);
		}
	}
}
