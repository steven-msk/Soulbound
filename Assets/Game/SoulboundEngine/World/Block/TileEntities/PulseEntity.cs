using SoulboundEngine.Client.World;
using SoulboundEngine.Common;
using SoulboundEngine.Core;
using SoulboundEngine.World.Block.State;

namespace SoulboundEngine.World.Block.Entity {
	[PROTOTYPICAL]
	public sealed class PulseEntity : TileEntity, ITickable {
		const int PULSE_INTERVAL = 40;
		const int PULSE_THRESHOLD = PULSE_INTERVAL - 1;
		private int timer;

		public PulseEntity(TileEntityType<PulseEntity> tileEntityType, BlockPos blockPos, BlockState blockState)
			: base(tileEntityType, blockPos, blockState) {
		}

		public static PulseEntity Create(BlockPos blockPos, BlockState blockState) {
			return new PulseEntity(TileEntityType.PULSE, blockPos, blockState);
		}

		public void Tick() {
			this.timer++;
			if (this.timer >= PULSE_INTERVAL) this.timer = 0;

			this.level.SetBlockState(this.blockPos, Blocks.PULSE_BLOCK.DefaultState.With(PulseBlock.on, this.timer == PULSE_THRESHOLD));
		}
	}
}
