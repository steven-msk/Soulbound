using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Core.Assets;
using SoulboundEngine.World.BlockSystem.Render;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.World.BlockSystem {
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
