using SoulboundEngine.Client.World.BlockSystem.Render;
using SoulboundEngine.Client.World.BlockSystem.States;

namespace SoulboundEngine.Client.World.BlockSystem {
	public sealed class AirBlock : Block {
		public AirBlock(Settings settings) : base(settings) {
		}

		public override BlockRenderData GetRenderData(BlockState blockState) {
			return new BlockRenderData(null);
		}
	}
}
