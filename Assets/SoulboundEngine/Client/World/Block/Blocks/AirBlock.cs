using SoulboundEngine.Client.World.BlockSystem.Render;
using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Core.Registry;

namespace SoulboundEngine.Client.World.BlockSystem {
	public sealed class AirBlock : Block {
		private static readonly Identifier identifier = new("air");
		public override string name { get; init; } = "Air";
		public override int minBreakLevel { get; init; } = 0;

		public AirBlock() : base(identifier) {
		}

		public override BlockRenderData GetRenderData(BlockState blockState) {
			return new BlockRenderData(null);
		}
	}
}
