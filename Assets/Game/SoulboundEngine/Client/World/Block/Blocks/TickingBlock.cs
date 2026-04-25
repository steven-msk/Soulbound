using SoulboundEngine.Client.World.BlockSystem.Render;
using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Common;
using SoulboundEngine.Core.Assets;
using SoulboundEngine.Core.State;
using SoulboundEngine.Core.States;

namespace SoulboundEngine.Client.World.BlockSystem {
	[PROTOTYPICAL]
	public sealed class TickingBlock : Block, ITickingBlock {
		public static readonly Property<bool> on = BoolProperty.Of("on");
		public static readonly Property<int> tickCount = IntProperty.OfRange("tickCount", 0, 19);

		public TickingBlock(Settings settings) 
			: base(settings) {
			this.SetDefaultState(this.DefaultState.With(on, false).With(tickCount, 0));
		}

		protected override void AppendProperties(StateManager<Block, BlockState>.Builder builder) {
			builder.Add(on, tickCount);
		}

		public void Tick(Level level, BlockPos blockPos, BlockState blockState) {
			int counter = blockState.Get(tickCount);
			counter++;
			
			bool on = blockState.Get(TickingBlock.on);
			if (counter >= 20) {
				on = !on;
				counter = 0;
			}

			BlockState state = this.DefaultState.With(TickingBlock.on, on).With(tickCount, counter);
			level.SetBlockState(blockPos, state);
		}

		public override BlockRenderData GetRenderData(BlockState blockState) {
			return new BlockRenderData(blockState.Get(on)
				? new AssetKey("TickBlockOn")
				: new AssetKey("TickBlockOff")
			);
		}
	}
}
