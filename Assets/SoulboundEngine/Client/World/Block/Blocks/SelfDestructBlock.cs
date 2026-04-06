using SoulboundEngine.Client.World.BlockSystem.Render;
using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Client.World.BlockSystem.TileEntities;
using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Common;
using SoulboundEngine.Core.Assets;
using SoulboundEngine.Core.Registry;

namespace SoulboundEngine.Client.World.BlockSystem {
	[PROTOTYPICAL]
	public sealed class SelfDestructBlock : Block {
		private static readonly Identifier identifier = Identifier.Of("self_destruct_block");
		public override string name { get; init; } = "Self Destruct Block";
		public override int minBreakLevel { get; init; } = 0;

		public SelfDestructBlock() : base(identifier) {
		}

		public override bool HasTileEntity(Level level, BlockPos blockPos, BlockState blockState) => true;

		public override TileEntity GetTileEntity(Level level, BlockPos blockPos) {
			return new SelfDestructEntity(level, blockPos);
		}

		public override BlockRenderData GetRenderData(BlockState blockState) {
			return new BlockRenderData(new AssetKey("RedSquareTile"));
		}
	}
}
