using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.World.BlockSystem.States;
using SoulboundBackend.Core.Assets;
using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

#nullable enable

namespace SoulboundBackend.Client.World.BlockSystem {
	public class GenericBlock : Block {
		public override string name { get; init; }
		public override int minBreakLevel { get; init; }

		private readonly AssetKey tileKey;

		public GenericBlock(
				string id,
				string name,
				AssetKey tileKey,
				int minBreakLevel
			) : base(id, name, minBreakLevel) {
			this.tileKey = tileKey;
		}

		public override AssetKey GetRenderTileKey(BlockState blockState) => tileKey;
	}
}
