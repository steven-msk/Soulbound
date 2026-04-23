using SoulboundEngine.Client.World.BlockSystem.Render;
using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Client.World.BlockSystem.TileEntities;
using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Common;
using SoulboundEngine.Core.Assets;

namespace SoulboundEngine.Client.World.BlockSystem {
	[PROTOTYPICAL]
	public sealed class PulseBlock : Block {
		public BlockState on { get; private set; }
		public BlockState off { get; private set; }
		public override string name { get; init; } = "Pulse Block";
		public override int minBreakLevel { get; init; } = 0;

		protected override void CreateStates(IBlockStateRegisterer registerer, BlockPropertyEntries properties) {
			on = registerer.AddWithProperties(properties.With("on", true));
			off = registerer.AddWithProperties(properties.With("on", false));
		}

		protected override BlockState GetDefaultState(IBlockStateRegisterer registerer, BlockPropertyEntries properties) {
			return off;
		}

		public override TileEntity GetTileEntity(Level level, BlockPos blockPos) {
			return new PulseEntity(level, blockPos);
		}

		public override bool HasTileEntity(Level level, BlockPos blockPos, BlockState blockState) {
			return true;
		}

		public override BlockRenderData GetRenderData(BlockState blockState) {
			return new BlockRenderData(blockState.Get<bool>("on")
				? new AssetKey("TickBlockOn")
				: new AssetKey("TickBlockOff")
			);
		}
	}
}
