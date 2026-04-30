using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Common;
using SoulboundEngine.Core;

namespace SoulboundEngine.Client.World.BlockSystem.TileEntities {
	[PROTOTYPICAL]
	public sealed class PulseEntity : TileEntity, ITickable {
		const int PULSE_INTERVAL = 40;
		const int PULSE_THRESHOLD = PULSE_INTERVAL - 1;
		private int timer;

		public PulseEntity(Level level, BlockPos blockPos)
			: base(level, blockPos) {
		}

		public void Tick() {
			this.timer++;
			if (this.timer >= PULSE_INTERVAL) this.timer = 0;

			this.level.SetBlockState(this.blockPos, Blocks.PULSE_BLOCK.DefaultState.With(PulseBlock.on, this.timer == PULSE_THRESHOLD));
		}
	}
}
