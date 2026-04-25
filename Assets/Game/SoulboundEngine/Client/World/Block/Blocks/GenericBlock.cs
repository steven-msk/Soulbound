using SoulboundEngine.Client.World.BlockSystem.Render;
using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Core.Assets;

#nullable enable

namespace SoulboundEngine.Client.World.BlockSystem {
	public class GenericBlock : Block {
		//public override string name { get; init; }
		//public override int minBreakLevel { get; init; }

		private readonly AssetKey tileKey;

		public GenericBlock(Settings settings, AssetKey tileKey) 
			: base(settings) {
			this.tileKey = tileKey;
		}

		public override BlockRenderData GetRenderData(BlockState blockState) {
			return new BlockRenderData(this.tileKey);
		}
	}
}
