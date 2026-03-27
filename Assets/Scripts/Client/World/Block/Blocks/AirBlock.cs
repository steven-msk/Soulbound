using SoulboundBackend.Client.World.BlockSystem.States;
using SoulboundBackend.World.BlockSystem.Render;
using UnityEngine;

namespace SoulboundBackend.Client.World.BlockSystem {
	public sealed class AirBlock : Block {
		public override string name { get; init; } = "Air";
		public override int minBreakLevel { get; init; } = 0;

		public AirBlock() : base("air") {
		}

		public override BlockRenderData GetRenderData(BlockState blockState) {
			return new BlockRenderData {
				tileKey = null,
				color = Color.white
			};
		}
	}
}
