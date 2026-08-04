using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Client.Interaction;
using SoulboundEngine.Client.Item;
using SoulboundEngine.Client.Player;
using SoulboundEngine.Client.World.Block.Entity;
using SoulboundEngine.Client.World.Block.State;

namespace SoulboundEngine.Client.World.Block {
	using Level = Level.Level;

	public class SignBlock : Block, ITileEntityProvider {
		public SignBlock(Settings settings) 
			: base(settings) {
		}

		public TileEntity CreateTileEntity(BlockPos pos, BlockState state) {
			return new SignTileEntity(pos, state);
		}

		protected override void OnHoverEnter(BlockState state, ItemStack stack, Level level, PlayerEntity player, BlockPos pos) {
			Logger.LogInfo("sign hover enter: {}", pos);
		}

		protected override void OnHoverLeave(BlockState state, ItemStack stack, Level level, PlayerEntity player, BlockPos pos) {
			Logger.LogInfo("sign hover leave: {}", pos);
		}

		protected override IActionResult OnSecondaryUse(BlockState state, Level level, PlayerEntity player, BlockPos pos) {
			Logger.LogInfo("sign set text triggered");
			return IActionResult.SUCCESS;
		}
	}
}
