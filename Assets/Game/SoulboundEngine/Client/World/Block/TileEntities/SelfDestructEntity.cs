using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Common;
using SoulboundEngine.Core;

namespace SoulboundEngine.Client.World.BlockSystem.TileEntities {
	[PROTOTYPICAL]
	public sealed class SelfDestructEntity : TileEntity, ITickable {
		const int TICKS_UNTIL_DESTRUCT = 300;
		private int timer = TICKS_UNTIL_DESTRUCT;

		public SelfDestructEntity(Level level, BlockPos blockPos)
			: base(level, blockPos) {
		}

		public void Tick() {
			this.timer--;

			if (this.timer <= 0) {
				this.level.SetBlockState(this.blockPos, Blocks.air.DefaultState);
			}
		}
	}
}
