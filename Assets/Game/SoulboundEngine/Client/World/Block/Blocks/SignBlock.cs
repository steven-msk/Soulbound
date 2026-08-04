using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Client.Item;
using SoulboundEngine.Client.Player;
using SoulboundEngine.Client.World.Block.State;

namespace SoulboundEngine.Client.World.Block {
	public class SignBlock : Block {
		public SignBlock(Settings settings) 
			: base(settings) {
		}

		protected override void OnHoverEnter(BlockState state, ItemStack stack, Level.Level level, PlayerEntity player, BlockPos pos) {
			Logger.LogInfo("sign hover enter: {}", pos);
		}

		protected override void OnHoverLeave(BlockState state, ItemStack stack, Level.Level level, PlayerEntity player, BlockPos pos) {
			Logger.LogInfo("sign hover leave: {}", pos);
		}
	}
}
