using SoulboundEngine.Client.World.BlockSystem.Render;
using SoulboundEngine.Client.World.BlockSystem.States;

namespace SoulboundEngine.Client.World.BlockSystem {
	public sealed class AirBlock : Block {
		public override string name { get; init; } = "Air";
		public override int minBreakLevel { get; init; } = 0;

		public override BlockRenderData GetRenderData(BlockState blockState) {
			return new BlockRenderData(null);
		}
	}
}
