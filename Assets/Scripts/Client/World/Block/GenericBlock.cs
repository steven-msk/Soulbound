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
		private readonly Func<BlockStateProperties?, IBlockStateBehavior>? behaviorFactory;
		private readonly Action<GenericBlock>? propertyRegisterer;
		private readonly Func<Block, BlockState>? defaultStateGetter;

		public GenericBlock(string name, TileBase tileReference, BlockItem itemReference)
			: this(name, tileReference, itemReference, null, null, null) { }

		public GenericBlock(
				string name,
				TileBase tileReference,
				BlockItem itemReference,
				Func<Block, BlockState>? defaultState = null,
				Func<BlockStateProperties?, IBlockStateBehavior>? behaviorFactory = null,
				Action<GenericBlock>? propertyRegisterer = null
			) {
			this.name = name;
			this.tileReference = tileReference;
			this.itemReference = itemReference;
			this.behaviorFactory = behaviorFactory;
			this.propertyRegisterer = propertyRegisterer;
			this.defaultStateGetter = defaultState;

			// Clears unwanted basic default state
			cachedStates.Clear();

			// Registers the actual default state
			RegisterProperties();
			RegisterDefaultState(CreateDefaultState());
		}

        protected override IBlockStateBehavior CreateBehaviorFor(BlockStateProperties properties) {
			return behaviorFactory?.Invoke(properties) ?? base.CreateBehaviorFor(properties);
        }

        protected override void RegisterProperties() {
			propertyRegisterer?.Invoke(this);			// null at initialization
        }

        protected override BlockState CreateDefaultState() {
			return defaultStateGetter?.Invoke(this)		// null at initialization
				?? new BlockState(this, null, CommonBlockBehaviors.Basic());
        }
    }
}
