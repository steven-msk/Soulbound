using SoulboundEngine.Client.World.BlockSystem.Render;
using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Core.Assets;
using SoulboundEngine.Core.States;

namespace SoulboundEngine.Client.World.BlockSystem {
	public class LeafBlock : Block {
		public static readonly Property<bool> persistent = BoolProperty.Of("persistent");

		public LeafBlock(Settings settings)
			: base(settings) {
			this.SetDefaultState(this.DefaultState.With(persistent, true));
		}

		protected override void AppendProperties(StateManager<Block, BlockState>.Builder builder) {
			builder.Add(persistent);
		}

		public override BlockRenderData GetRenderData(BlockState blockState) {
			return new BlockRenderData(new AssetKey("leaves"));
		}
	}
}
