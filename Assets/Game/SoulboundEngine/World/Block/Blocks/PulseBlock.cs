using SoulboundEngine.Client.World.Block.Entity;
using SoulboundEngine.Client.World.Block.State;
using SoulboundEngine.Common;
using SoulboundEngine.Core.States;

namespace SoulboundEngine.Client.World.Block {
	[PROTOTYPICAL]
	public sealed class PulseBlock : Block, ITileEntityProvider {
		public static readonly Property<bool> on = BoolProperty.Of("on");

		public PulseBlock(AbstractBlock.Settings settings) 
			: base(settings) {
			this.SetDefaultState(this.DefaultState.With(on, false));
		}

		protected override void AppendProperties(StateManager<Block, BlockState>.Builder builder) {
			builder.Add(on);
		}

		public TileEntity CreateTileEntity(BlockPos pos, BlockState state) {
			return PulseEntity.Create(pos, state);
		}
	}
}
