using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Common;
using SoulboundEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.World.BlockSystem.TileEntities {
	[PROTOTYPICAL]
	public sealed class SelfDestructEntity : TileEntity, ITickable {
		const int TICKS_UNTIL_DESTRUCT = 300;
		private int timer = TICKS_UNTIL_DESTRUCT;

		public SelfDestructEntity(Level level, BlockPos blockPos)
			: base(level, blockPos) {
		}

		public void Tick() {
			timer--;

			if (timer <= 0) {
				level.SetBlockState(blockPos, Blocks.air.defaultState);
			}
		}
	}
}
