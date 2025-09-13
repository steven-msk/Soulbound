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
		public override BlockState defaultState { get; }
		private Func<Dictionary<string, object>?, IBlockStateBehavior>? behaviorFactory { get; }

		public GenericBlock(string name, TileBase tileReference, BlockItem itemReference)
			: this(name, tileReference, itemReference, null, _ => BlockBehaviors.PassiveBehavior()) { }

		public GenericBlock(string name, TileBase tileReference, BlockItem itemReference, Func<Dictionary<string, object>?, IBlockStateBehavior> behaviorFactory)
			: this(name, tileReference, itemReference, null, behaviorFactory) { }

		public GenericBlock(string name, TileBase tileReference, BlockItem itemReference, Dictionary<string, object>? defaultProperties = null,
				Func<Dictionary<string, object>?, IBlockStateBehavior>? behaviorFactory = null) {
			this.name = name;
			this.tileReference = tileReference;
			this.behaviorFactory = behaviorFactory;
			IBlockStateBehavior defaultBehavior = GetFromNullable(defaultProperties);
			this.defaultState = new BlockState(this, defaultProperties ?? new Dictionary<string, object>(), defaultBehavior);
			this.itemReference = itemReference;
		}

		public override BlockState CreateState(Dictionary<string, object>? properties) {
			IBlockStateBehavior behavior = GetFromNullable(properties);
			return new BlockState(this, properties, behavior);
		}

		private IBlockStateBehavior GetFromNullable(Dictionary<string, object>? properties) {
			return behaviorFactory?.Invoke(properties ?? new Dictionary<string, object>()) ?? BlockBehaviors.NoBehavior();
		}
	}
}
