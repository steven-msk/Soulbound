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
	public sealed class PulsateTickingBlock : Block, ITickingBlock {
		private BlockState[] states;	// [counter={0..100}]
		public override string name { get; init; } = "Pulsate Ticking Block";
		public override BlockItem itemReference { get; init; }

		public PulsateTickingBlock() : base("pulsateTickingBlock") {
		}

		public override AssetKey GetRenderTileKey(BlockState blockState) {
			return blockState.Get<int>("counter") == 99
				? new AssetKey("TickBlockOn")
				: new AssetKey("TickBlockOff");
		}

		public void Tick(Level level, BlockPos blockPos, BlockState blockState) {
			int counter = blockState.Get<int>("counter");
			counter++;
			
			if (counter >= 100) counter = 0;
			level.SetBlockState(blockPos, states[counter]);
		}

		protected override void CreateStates(BlockStateRegisterer registerer, BlockPropertyEntries properties) {
			states = new BlockState[100];

			for (int i = 0; i < 100; i++) {
				states[i] = registerer.AddWithProperties(properties.With("counter", i));
			}
		}

		protected override BlockState GetDefaultState(BlockStateRegisterer registerer, BlockPropertyEntries properties) {
			return states[0];
		}
	}
}
