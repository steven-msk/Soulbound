using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Client.Interaction;
using SoulboundEngine.Client.World.Block.State;
using SoulboundEngine.Core.States;

namespace SoulboundEngine.Client.World.Block {
	public sealed class ToggleBlock : Block, IInteractableBlock {
		public static readonly Property<bool> on = BoolProperty.Of("on");

		public ToggleBlock(Settings settings) 
			: base(settings) {
			this.SetDefaultState(this.DefaultState.With(on, false));
		}

		protected override void AppendProperties(StateManager<Block, BlockState>.Builder builder) {
			builder.Add(on);
		}

		public void OnInteract(in BlockInteractionResult ctx) {
			bool isOn = ctx.blockState.Get(on);
			isOn = !isOn;
			ctx.level.SetBlockState(ctx.blockPos, this.DefaultState.With(on, isOn));
			Logger.LogInfo("block at {} is now {}", ctx.blockPos, isOn ? "off" : "on");
		}

		public bool CanInteract(in BlockInteractionResult ctx) => true;

		public bool ValidateTrigger(InteractionTrigger trigger) {
			return trigger == InteractionTrigger.RightClick;
		}
	}
}
