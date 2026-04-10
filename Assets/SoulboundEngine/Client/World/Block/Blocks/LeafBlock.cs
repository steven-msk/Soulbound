using SoulboundEngine.Client.World.BlockSystem.Render;
using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Core.Assets;

namespace SoulboundEngine.Client.World.BlockSystem {
	public class LeafBlock : Block {
		public override string name { get; init; } = "Leaves";
		public override int minBreakLevel { get; init; } = 0;

		public override BlockRenderData GetRenderData(BlockState blockState) {
			return new BlockRenderData(new AssetKey("leaves"));
		}

		protected override BlockState GetDefaultState(IBlockStateRegisterer registerer, BlockPropertyEntries propertyEntries) {
			return registerer.AddWithProperties(propertyEntries.With("persistent", true));
		}
	}
}
