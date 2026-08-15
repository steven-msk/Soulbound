using SoulboundEngine.Client.World.Block.State;
using SoulboundEngine.Common;
using SoulboundEngine.Core;

namespace SoulboundEngine.Client.World.Block.Entity {
	[PROTOTYPICAL]
	public sealed class SelfDestructEntity : TileEntity, ITickable {
		const int TICKS_UNTIL_DESTRUCT = 300;
		private int timer = TICKS_UNTIL_DESTRUCT;

		public SelfDestructEntity(TileEntityType<SelfDestructEntity> tileEntityType, BlockPos blockPos, BlockState blockState)
			: base(tileEntityType, blockPos, blockState) {
		}

		public static SelfDestructEntity Create(BlockPos blockPos, BlockState blockState) {
			return new SelfDestructEntity(TileEntityType.SELF_DESTRUCT_BLOCK, blockPos, blockState);
		}

		public void Tick() {
			this.timer--;

			if (this.timer <= 0) {
				this.level.SetBlockState(this.blockPos, Blocks.AIR.DefaultState);
			}
		}
	}
}
