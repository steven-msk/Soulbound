using SoulboundBackend.Client.World.BlockSystem.States;
using SoulboundBackend.Core.Assets;
using SoulboundBackend.World.BlockSystem.Render;
using UnityEngine;

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

		public override BlockRenderData GetRenderData(BlockState blockState) {
			return new BlockRenderData {
				tileKey = this.tileKey,
				color = Color.white
			};
		}
	}
}
