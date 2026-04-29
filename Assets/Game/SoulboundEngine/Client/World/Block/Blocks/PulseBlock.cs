using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Client.World.BlockSystem.TileEntities;
using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Common;
using SoulboundEngine.Core.States;

namespace SoulboundEngine.Client.World.BlockSystem {
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

		public override TileEntity GetTileEntity(Level level, BlockPos blockPos) {
			return new PulseEntity(level, blockPos);
		}

		public override bool HasTileEntity(Level level, BlockPos blockPos, BlockState blockState) {
			return true;
		}

	}
}
