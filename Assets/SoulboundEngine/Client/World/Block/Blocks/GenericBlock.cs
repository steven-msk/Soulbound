using SoulboundEngine.Client.World.BlockSystem.Render;
using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Core.Assets;
using SoulboundEngine.Core.Registry;

#nullable enable

namespace SoulboundEngine.Client.World.BlockSystem {
	public class GenericBlock : Block {
		public override string name { get; init; }
		public override int minBreakLevel { get; init; }

		private readonly AssetKey tileKey;

		public GenericBlock(
				Identifier identifier,
				string name,
				AssetKey tileKey,
				int minBreakLevel
			) : base(identifier, name, minBreakLevel) {
			this.tileKey = tileKey;
		}

		public override BlockRenderData GetRenderData(BlockState blockState) {
			return new BlockRenderData(tileKey);
		}
	}
}
