using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.World.BlockSystem.States;
using SoulboundBackend.Client.World.LevelDomain;
using SoulboundBackend.Common;
using SoulboundBackend.Core.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.BlockSystem {
	[PROTOTYPICAL]
	public sealed class TickingBlock : Block, ITickingBlock {
		private BlockState[,] states;	// [on=1/off=0, counter={0..19}]
		public override string name { get; init; } = "Ticking Block";
		public override int minBreakLevel { get; init; } = 0;

		public TickingBlock() : base("tickingBlock") {
		}

		public override AssetKey GetRenderTileKey(BlockState blockState) {
			return blockState.Get<bool>("on")
				? new AssetKey("TickBlockOn")
				: new AssetKey("TickBlockOff");
		}

		public void Tick(Level level, BlockPos blockPos, BlockState blockState) {
			int counter = blockState.Get<int>("counter");
			counter++;
			
			bool on = blockState.Get<bool>("on");
			if (counter >= 20) {
				on = !on;
				counter = 0;
			}

			level.SetBlockState(blockPos, states[on ? 1 : 0, counter]);
		}

		protected override void CreateStates(BlockStateRegisterer registerer, BlockPropertyEntries properties) {
			states = new BlockState[2, 20];
			for (int on = 0; on <= 1; on++) {
				for (int counter = 0; counter < 20; counter++) {
					BlockState blockState = registerer.AddWithProperties(
						properties
							.With("on", on == 1)
							.With("counter", counter)
					);
					states[on, counter] = blockState;
				}
			}
		}

		protected override BlockState GetDefaultState(BlockStateRegisterer registerer, BlockPropertyEntries properties) {
			return states[0, 0];
		}

	}
}
