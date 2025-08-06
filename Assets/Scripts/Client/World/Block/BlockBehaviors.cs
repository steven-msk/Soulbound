using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public static class BlockBehaviors {
    public delegate Vector2 DropForce();
    public delegate List<ItemStack> DropsGetter(BlockState blockState, BreakSource breakSource);
    public delegate void OnNeighborStateChanged(BlockPos selfPos, BlockPos neighborPos, BlockState oldState, BlockState newState);
    public delegate void OnPlace(BlockPos blockPos, BlockState blockState);

    public static DropForce DefaultDropForce => () => new(Random.Range(-1f, 1f), Random.Range(2.5f, 3f));
    public static OnNeighborStateChanged NoNeighborUpdate => (_, _, _, _) => { };
    public static OnPlace NoPlaceAction => (_, _) => { };

    public static IBlockStateBehavior NoBehavior() {
        return FromDelegates(
            dropForce: () => default,
            dropsGetter: (blockState, breakSource) => new List<ItemStack>(),
            NoNeighborUpdate,
            NoPlaceAction
        );
    }

    public static IBlockStateBehavior PassiveBehavior() {
        return FromDelegates(
            DefaultDropForce,
            dropsGetter: (blockState, breakSource) => new List<ItemStack>() { new ItemStack(blockState.block.itemReference, 1) },
            NoNeighborUpdate,
            NoPlaceAction
        );
    }

    public static IBlockStateBehavior DropIfPlayerBroke() {
        return FromDelegates(
            DefaultDropForce,
            dropsGetter: (blockState, breakSource) => {
                List<ItemStack> list = new List<ItemStack>();
                if (breakSource == BreakSource.Player) {
                    list.Add(new ItemStack(blockState.block.itemReference, 1));
                }
                return list;
            },
            NoNeighborUpdate,
            NoPlaceAction
        );
    }

    public static IBlockStateBehavior FromDelegates(DropForce dropForce, DropsGetter dropsGetter, OnNeighborStateChanged onNeighborStateChanged, OnPlace onPlace) {
        return new DelegateStateBehavior(dropForce, dropsGetter, onNeighborStateChanged, onPlace);
    }

    public class DelegateStateBehavior : IBlockStateBehavior {
        private DropForce dropForce;
        private DropsGetter dropsGetter;
        private OnNeighborStateChanged onNeighborStateChanged;
        private OnPlace onPlace;

        public DelegateStateBehavior(DropForce dropForce, DropsGetter dropsGetter, OnNeighborStateChanged onNeighborStateChanged, OnPlace onPlace) {
            this.dropForce = dropForce;
            this.dropsGetter = dropsGetter;
            this.onNeighborStateChanged = onNeighborStateChanged;
            this.onPlace = onPlace;
        }

        public List<ItemStack> GetDrops(BlockState blockState, BreakSource source) {
            return dropsGetter.Invoke(blockState, source);
        }

        public void OnNeighborStateChanged(BlockPos selfPos, BlockPos neighborPos, BlockState oldState, BlockState newState) {
            onNeighborStateChanged.Invoke(selfPos, neighborPos, oldState, newState);
        }

        public void OnPlace(BlockPos blockPos, BlockState blockState) {
            onPlace.Invoke(blockPos, blockState);
        }
    }
}
