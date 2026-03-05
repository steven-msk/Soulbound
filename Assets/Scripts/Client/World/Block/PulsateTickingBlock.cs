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
		private const int PULSE_INTERVAL = 40;
		private BlockState[] states;	// [counter={0..(PULSE_INTERVAL - 1)}]
		public override string name { get; init; } = "Pulsate Ticking Block";
		public override BlockItem itemReference { get; init; }

		public PulsateTickingBlock() : base("pulsateTickingBlock") {
		}

		public override AssetKey GetRenderTileKey(BlockState blockState) {
			return blockState.Get<int>("counter") == PULSE_INTERVAL - 1
				? new AssetKey("TickBlockOn")
				: new AssetKey("TickBlockOff");
		}

		public void Tick(Level level, BlockPos blockPos, BlockState blockState) {
			int counter = blockState.Get<int>("counter");
			counter++;
			
			if (counter >= PULSE_INTERVAL) counter = 0;
			level.SetBlockState(blockPos, states[counter]);
		}

		protected override void CreateStates(BlockStateRegisterer registerer, BlockPropertyEntries properties) {
			states = new BlockState[PULSE_INTERVAL];

			for (int i = 0; i < PULSE_INTERVAL; i++) {
				states[i] = registerer.AddWithProperties(properties.With("counter", i));
			}
		}

		protected override BlockState GetDefaultState(BlockStateRegisterer registerer, BlockPropertyEntries properties) {
			return states[0];
		}
	}
}
