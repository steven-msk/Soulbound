using SoulboundBackend.Client.ItemSystem;
using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

#nullable enable

namespace SoulboundBackend.Client.World.BlockSystem {
	public class GenericBlock : Block {
		public override string name { get; }
		public override TileBase tileReference { get; }
		public override BlockItem? itemReference { get; }
		public override BreakRequirement? breakRequirement { get; }
		private readonly Func<BlockStateProperties?, IBlockStateBehavior>? behaviorFactory;
		private readonly Action<GenericBlock>? propertyRegisterer;
		private readonly Func<Block, BlockState>? defaultStateGetter;
		private readonly Func<Block, IReadOnlyList<BlockState>>? stateInitializer;
		private readonly Func<Block, ItemStack, BlockPos, BlockState>? placeFunction;

		public GenericBlock(
				string name, 
				TileBase tileReference,
				BlockItem itemReference, 
				IBlockStateCacheStrategy stateCacheStrategy,
				BreakRequirement? breakRequirement
			)
			: this(name,
				   tileReference, 
				   itemReference,
				   stateCacheStrategy,
				   breakRequirement, 
				   null, null, null, null
			) { 
		}

		public GenericBlock(
				string name,
				TileBase tileReference,
				BlockItem itemReference,
				IBlockStateCacheStrategy stateCacheStrategy,
				BreakRequirement? breakRequirement,
				Func<Block, BlockState>? defaultState = null,
				Func<BlockStateProperties?, IBlockStateBehavior>? behaviorFactory = null,
				Action<GenericBlock>? propertyRegisterer = null,
				Func<Block, IReadOnlyList<BlockState>>? stateInitializer = null,
				Func<Block, ItemStack, BlockPos, BlockState>? placeFunction = null
			) : base(stateCacheStrategy) {
			this.name = name;
			this.tileReference = tileReference;
			this.itemReference = itemReference;
			this.breakRequirement = breakRequirement;
			this.behaviorFactory = behaviorFactory;
			this.propertyRegisterer = propertyRegisterer;
			this.defaultStateGetter = defaultState;
			this.stateInitializer = stateInitializer;
			this.placeFunction = placeFunction;

			// Clear unwanted basic default state
			propertyDefinitionTerminated = false;
			if (stateCacheStrategy is IBlockStateCacheResettable resettable) {
				resettable.ResetCache();
			}

			// Register the actual default state
			RegisterProperties();
			RegisterDefaultState(CreateDefaultState());
			stateCacheStrategy.Initialize(this);
		}

		public override IBlockStateBehavior CreateBehaviorFor(BlockStateProperties properties) {
			return behaviorFactory?.Invoke(properties) ?? base.CreateBehaviorFor(properties);
		}

		protected override void RegisterProperties() {
			propertyRegisterer?.Invoke(this);			// null at initialization
		}

		protected override BlockState CreateDefaultState() {
			return defaultStateGetter?.Invoke(this)		// null at initialization
				?? new BlockState(this, null, CommonBlockBehaviors.DropSingle());
		}

		public override bool GetPredefinedStates(out IReadOnlyList<BlockState> states) {
			states = stateInitializer?.Invoke(this) ?? new List<BlockState>();
			return states.Count > 0;
		}

        public override BlockState Place(ItemStack itemStack, BlockPos blockPos) {
            return placeFunction?.Invoke(this, itemStack, blockPos) ?? base.Place(itemStack, blockPos);
        }
	}
}
