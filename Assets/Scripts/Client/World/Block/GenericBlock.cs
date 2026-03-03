using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Core.AssetManagement;
using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

#nullable enable

namespace SoulboundBackend.Client.World.BlockSystem {
	public class GenericBlock : Block {
		public override string name { get; init; }
		//public override TileBase tileReference { get; init; }
		public override BlockItem itemReference { get; init; }
		public override BreakRequirement? breakRequirement { get; init; }
		public override AssetKey tileKey { get; init; }
		// proteted

		public GenericBlock(
				string id,
				string name,
				//TileBase tileReference,
				AssetKey tileKey,
				BlockItem itemReference,
				BreakRequirement? breakRequirement
			//) : base(id, name, tileReference, itemReference, breakRequirement) {
			) : base(id, name, tileKey, itemReference, breakRequirement) {
		}

		public override IEnumerable<ItemStack> GetDrops(BlockState blockState, BreakSource source) {
			yield return new(itemReference, 1);
		}
	}
}
