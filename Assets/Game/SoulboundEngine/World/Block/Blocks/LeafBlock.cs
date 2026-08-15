using SoulboundEngine.World.Block.State;
using SoulboundEngine.Core.States;

namespace SoulboundEngine.World.Block {
	public class LeafBlock : Block {
		public static readonly Property<bool> persistent = BoolProperty.Of("persistent");

		public LeafBlock(AbstractBlock.Settings settings)
			: base(settings) {
			this.SetDefaultState(this.DefaultState.With(persistent, true));
		}

		protected override void AppendProperties(StateManager<Block, BlockState>.Builder builder) {
			builder.Add(persistent);
		}
	}
}
