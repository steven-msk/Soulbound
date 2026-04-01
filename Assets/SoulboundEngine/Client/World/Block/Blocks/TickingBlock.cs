using SoulboundEngine.Client.World.BlockSystem.Render;
using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Common;
using SoulboundEngine.Core.Assets;
using SoulboundEngine.Core.Registry;

namespace SoulboundEngine.Client.World.BlockSystem {
	[PROTOTYPICAL]
	public sealed class TickingBlock : Block, ITickingBlock {
		private static readonly Identifier identifier = new("tickingBlock");
		private BlockState[,] states;	// [on=1/off=0, counter={0..19}]
		public override string name { get; init; } = "Ticking Block";
		public override int minBreakLevel { get; init; } = 0;

		public TickingBlock() : base(identifier) {
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

		protected override void CreateStates(IBlockStateRegisterer registerer, BlockPropertyEntries properties) {
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

		protected override BlockState GetDefaultState(IBlockStateRegisterer registerer, BlockPropertyEntries properties) {
			return states[0, 0];
		}

		public override BlockRenderData GetRenderData(BlockState blockState) {
			return new BlockRenderData(blockState.Get<bool>("on")
				? new AssetKey("TickBlockOn")
				: new AssetKey("TickBlockOff")
			);
		}
	}
}
