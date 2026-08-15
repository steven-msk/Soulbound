using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Interaction;
using SoulboundEngine.Client.Player;
using SoulboundEngine.Client.World.Block.State;
using SoulboundEngine.Core.States;

namespace SoulboundEngine.Client.World.Block {
	public sealed class ToggleBlock : Block {
		public static readonly Property<bool> on = BoolProperty.Of("on");

		public ToggleBlock(AbstractBlock.Settings settings) 
			: base(settings) {
			this.SetDefaultState(this.DefaultState.With(on, false));
		}

		protected override void AppendProperties(StateManager<Block, BlockState>.Builder builder) {
			builder.Add(on);
		}

		protected override IActionResult OnSecondaryUse(BlockState state, Level.Level level, PlayerEntity player, BlockPos pos) {
			bool isOn = state.Get(on);
			isOn = !isOn;
			level.SetBlockState(pos, this.DefaultState.With(on, isOn));
			Logger.LogInfo("block at {} is now {}", pos, isOn ? "off" : "on");
			return IActionResult.SUCCESS;
		}
	}
}
