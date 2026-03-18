using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Common;
using SoulboundBackend.Core.AssetManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.BlockSystem {
	[PROTOTYPICAL]
	public sealed class SelfDestructBlock : Block {
		public override string name { get; init; } = "Self Destruct Block";
		public override int minBreakLevel { get; init; } = 0;

		public SelfDestructBlock() : base("selfDestructBlock") {
		}

		public override AssetKey GetRenderTileKey(BlockState blockState) => new("RedSquareTile");

		public override bool HasTileEntity(Level level, BlockPos blockPos, BlockState blockState) => true;

		public override TileEntity GetTileEntity(Level level, BlockPos blockPos) {
			return new SelfDestructEntity(level, blockPos);
		}
	}
}
