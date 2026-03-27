using SoulboundBackend.Client.World.BlockSystem.States;
using SoulboundBackend.Client.World.BlockSystem.TileEntities;
using SoulboundBackend.Client.World.LevelDomain;
using SoulboundBackend.Common;
using SoulboundBackend.Core.Assets;
using SoulboundBackend.World.BlockSystem.Render;

namespace SoulboundBackend.Client.World.BlockSystem {
	[PROTOTYPICAL]
	public sealed class SelfDestructBlock : Block {
		public override string name { get; init; } = "Self Destruct Block";
		public override int minBreakLevel { get; init; } = 0;

		public SelfDestructBlock() : base("selfDestructBlock") {
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
