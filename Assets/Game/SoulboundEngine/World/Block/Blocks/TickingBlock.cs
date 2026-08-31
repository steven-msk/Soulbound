namespace SoulboundEngine.World.Block {
	using SoulboundEngine.Common;
	using SoulboundEngine.State;
	using SoulboundEngine.World.Block.State;
	using SoulboundEngine.World.Level;

	[PROTOTYPICAL]
	public sealed class TickingBlock : Block, ITickingBlock {
		public static readonly Property<bool> on = BoolProperty.Of("on");
		public static readonly Property<int> tickCount = IntProperty.OfRange("tickCount", 0, 19);

		public TickingBlock(AbstractBlock.Settings settings) 
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

	}
}
