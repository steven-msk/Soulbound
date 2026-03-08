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
	public sealed class PulseBlock : Block {
		public BlockState on { get; private set; }
		public BlockState off { get; private set; }
		public override string name { get; init; } = "Pulse Block";

		public PulseBlock() : base("pulseBlock") {
		}

		public override AssetKey GetRenderTileKey(BlockState blockState) {
			return blockState.Get<bool>("on")
				? new AssetKey("TickBlockOn")
				: new AssetKey("TickBlockOff");
		}

		protected override void CreateStates(BlockStateRegisterer registerer, BlockPropertyEntries properties) {
			on = registerer.AddWithProperties(properties.With("on", true));
			off = registerer.AddWithProperties(properties.With("on", false));
		}

		protected override BlockState GetDefaultState(BlockStateRegisterer registerer, BlockPropertyEntries properties) {
			return off;
		}

		public override TileEntity GetTileEntity(Level level, BlockPos blockPos) {
			return new PulseEntity(level, blockPos);
		}

		public override bool HasTileEntity(Level level, BlockPos blockPos, BlockState blockState) {
			return true;
		}
	}
}
