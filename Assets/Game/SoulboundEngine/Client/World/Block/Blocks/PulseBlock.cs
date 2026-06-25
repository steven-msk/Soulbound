using SoulboundEngine.Client.World.Block.State;
using SoulboundEngine.Client.World.Block.TileEntity;
using SoulboundEngine.Client.World.Level;
using SoulboundEngine.Common;
using SoulboundEngine.Core.States;

namespace SoulboundEngine.Client.World.Block {
	[PROTOTYPICAL]
	public sealed class PulseBlock : Block {
		public static readonly Property<bool> on = BoolProperty.Of("on");

		public PulseBlock(Settings settings) 
			: base(settings) {
			this.SetDefaultState(this.DefaultState.With(on, false));
		}

		protected override void AppendProperties(StateManager<Block, BlockState>.Builder builder) {
			builder.Add(on);
		}

		public override TileEntity.TileEntity GetTileEntity(Level.Level level, BlockPos blockPos) {
			return new PulseEntity(TileEntityTypes.PULSE, level, blockPos);
		}

		public override bool HasTileEntity(Level.Level level, BlockPos blockPos, BlockState blockState) {
			return true;
		}

	}
}
