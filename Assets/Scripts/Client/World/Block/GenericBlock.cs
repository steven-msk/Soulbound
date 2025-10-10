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
		private Func<Dictionary<string, object>?, IBlockStateBehavior>? behaviorFactory;
		private Action<GenericBlock>? propertyRegisterer;

		public GenericBlock(string name, TileBase tileReference, BlockItem itemReference)
			: this(name, tileReference, itemReference, null, null, null) { }

		public GenericBlock(
				string name,
				TileBase tileReference,
				BlockItem itemReference,
				Dictionary<string, object>? defaultProperties,
				Func<Dictionary<string, object>?, IBlockStateBehavior>? behaviorFactory = null,
				Action<GenericBlock>? propertyRegisterer = null
			) {
			this.name = name;
			this.tileReference = tileReference;
			this.itemReference = itemReference;
			this.behaviorFactory = behaviorFactory;
			this.propertyRegisterer = propertyRegisterer;

			this.RegisterProperties();
			defaultProperties ??= new Dictionary<string, object>();
			var defaultBehavior = behaviorFactory?.Invoke(defaultProperties)
				?? CommonBlockBehaviors.Basic();
			RegisterDefaultState(new BlockState(this, defaultProperties, defaultBehavior));
		}

        protected override IBlockStateBehavior CreateBehaviorFor(Dictionary<string, object> properties) {
			return behaviorFactory?.Invoke(properties) ?? base.CreateBehaviorFor(properties);
        }

        public override void RegisterProperties() {
			propertyRegisterer?.Invoke(this);
        }
    }
}
