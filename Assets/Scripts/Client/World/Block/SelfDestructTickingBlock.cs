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
	public sealed class SelfDestructTickingBlock : Block, ITickingBlock {
		const int TIME_UNTIL_DESTRUCT = 300;
		private BlockState[] states;	// [timer={0..(TIME_UNTIL_DESTRUCT - 1)}]
		public override string name { get; init; } = "Self Destruct Ticking Block";
		public override BlockItem itemReference { get; init; }

		public SelfDestructTickingBlock() : base("selfDestructTickingBlock") {
		}

		public override AssetKey GetRenderTileKey(BlockState blockState) => new("RedSquareTile");

		public void Tick(Level level, BlockPos blockPos, BlockState blockState) {
			int timer = blockState.Get<int>("timer");
			timer--;

			level.SetBlockState(blockPos, timer < 0
				? Blocks.air.defaultState
				: states[timer]);
		}

		protected override void CreateStates(BlockStateRegisterer registerer, BlockPropertyEntries properties) {
			states = new BlockState[TIME_UNTIL_DESTRUCT];

			for (int i = 0; i < TIME_UNTIL_DESTRUCT; i++) {
				states[i] = registerer.AddWithProperties(properties.With("timer", i));
			}
		}

		protected override BlockState GetDefaultState(BlockStateRegisterer registerer, BlockPropertyEntries properties) {
			return states[TIME_UNTIL_DESTRUCT - 1];
		}
	}
}
