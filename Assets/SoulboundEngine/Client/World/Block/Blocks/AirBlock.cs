using SoulboundEngine.Client.World.BlockSystem.Render;
using SoulboundEngine.Client.World.BlockSystem.States;
using UnityEngine;

namespace SoulboundEngine.Client.World.BlockSystem {
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
