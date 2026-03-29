namespace SoulboundEngine.Client.World.BlockSystem.Render {
	public sealed class BlockModelResolver {
		public BlockRenderModel ResolveModel(BlockRenderData renderData) {
			return new BlockRenderModel {
				tileKey = renderData.tileKey,
				color = renderData.color
			};
		}
	}
}
