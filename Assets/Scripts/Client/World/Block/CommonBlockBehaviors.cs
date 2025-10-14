using SoulboundBackend.Client.ItemSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SoulboundBackend.Client.World.BlockSystem {
	public static class CommonBlockBehaviors {
		public delegate Vector2 DropForce();
		public delegate List<ItemStack> DropsGetter(BlockState blockState, BreakSource breakSource);
		public delegate void OnNeighborStateChanged(BlockPos selfPos, BlockPos neighborPos, BlockState oldState, BlockState newState);
		public delegate void OnPlace(BlockPos blockPos, BlockState blockState);

		public static DropForce DefaultDropForce => () => new(Random.Range(-1f, 1f), Random.Range(2.5f, 3f));
		public static OnNeighborStateChanged NoNeighborUpdate => (_, _, _, _) => { };
		public static OnPlace NoPlaceAction => (_, _) => { };

		public static IBlockStateBehavior NullBehavior() {
			return FromDelegates(
				dropForce: () => default,
				dropsGetter: (blockState, breakSource) => new List<ItemStack>(),
				NoNeighborUpdate,
				NoPlaceAction
			);
		}

		public static IBlockStateBehavior DropSingle() {
			return FromDelegates(
				DefaultDropForce,
				dropsGetter: (blockState, breakSource) => 
					new List<ItemStack>() { new ItemStack(blockState.block.itemReference, 1) },
				NoNeighborUpdate,
				NoPlaceAction
			);
		}

		public static IBlockStateBehavior DropSingleIfBrokenByPlayer() {
			return DropSingleIf((blockState, breakSource) => breakSource is PlayerToolBreakSource);
		}

		public static IBlockStateBehavior DropSingleIf(Func<BlockState, BreakSource, bool> predicate) {
			return FromDelegates(
				DefaultDropForce,
				dropsGetter: (blockState, breakSource) => {
					List<ItemStack> drops = new List<ItemStack>();
					if (predicate.Invoke(blockState, breakSource)) {
						drops.Add(new ItemStack(blockState.block.itemReference, 1));
					}
					return drops;
				},
				NoNeighborUpdate,
				NoPlaceAction
			);
		}

		public static IBlockStateBehavior FromDelegates(
				DropForce dropForce, 
				DropsGetter dropsGetter, 
				OnNeighborStateChanged onNeighborStateChanged,
				OnPlace onPlace
			) {
			return new DelegateStateBehavior(dropForce, dropsGetter, onNeighborStateChanged, onPlace);
		}

		private class DelegateStateBehavior : IBlockStateBehavior {
			private DropForce dropForce;
			private DropsGetter dropsGetter;
			private OnNeighborStateChanged onNeighborStateChanged;
			private OnPlace onPlace;

			public DelegateStateBehavior(
					DropForce dropForce, 
					DropsGetter dropsGetter,
					OnNeighborStateChanged onNeighborStateChanged, 
					OnPlace onPlace
				) {
				this.dropForce = dropForce;
				this.dropsGetter = dropsGetter;
				this.onNeighborStateChanged = onNeighborStateChanged;
				this.onPlace = onPlace;
			}

			public List<ItemStack> GetDrops(BlockState blockState, BreakSource receiver) {
				return dropsGetter.Invoke(blockState, receiver);
			}

			public void OnNeighborStateChanged(BlockPos selfPos, BlockPos neighborPos, BlockState oldState, BlockState newState) {
				onNeighborStateChanged.Invoke(selfPos, neighborPos, oldState, newState);
			}

			public void OnPlace(BlockPos blockPos, BlockState blockState) {
				onPlace.Invoke(blockPos, blockState);
			}
		}
	}
}
