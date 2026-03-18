using SoulboundBackend.Common;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.BlockSystem {
	[PROTOTYPICAL]
	public sealed class PulseEntity : TileEntity, ITickable {
		const int PULSE_INTERVAL = 40;
		const int PULSE_THRESHOLD = PULSE_INTERVAL - 1;
		private int timer;

		public PulseEntity(Level level, BlockPos blockPos)
			: base(level, blockPos) {
		}

		public void Tick() {
			timer++;
			if (timer >= PULSE_INTERVAL) timer = 0;

			level.SetBlockState(blockPos, timer == PULSE_THRESHOLD
				? Blocks.pulseBlock.on
				: Blocks.pulseBlock.off
			);
		}
	}
}
