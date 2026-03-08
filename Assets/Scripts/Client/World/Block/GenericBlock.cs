using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Core.AssetManagement;
using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

#nullable enable

namespace SoulboundBackend.Client.World.BlockSystem {
	public class GenericBlock : Block {
		public override string name { get; init; }
		public override BreakRequirement? breakRequirement { get; init; }
		private readonly AssetKey tileKey;

		public GenericBlock(
				string id,
				string name,
				AssetKey tileKey,
				BreakRequirement? breakRequirement
			) : base(id, name, breakRequirement) {
			this.tileKey = tileKey;
		}

		//public override IEnumerable<ItemStack> GetDrops(BlockState blockState, BreakSource source) {
		//	yield return new(itemReference, 1);
		//}

		public override AssetKey GetRenderTileKey(BlockState blockState) => tileKey;
	}
}
