using SoulboundEngine.Client.Interaction;
using SoulboundEngine.Client.Player;
using SoulboundEngine.Client.World.Block;
using SoulboundEngine.Client.World.Block.State;
using SoulboundEngine.Client.World.Level;

#nullable enable

namespace SoulboundEngine.Client.Item {
	public class BlockItem : Item {
		private readonly Block block;

		public BlockItem(Block block, Settings settings) 
			: base(settings) {
			this.block = block;
			this.AppendToBlock(block);
		}

		public Block GetBlock() => this.block;

		public override IActionResult OnPrimaryUse(ItemStack stack, Level level, PlayerEntity player, BlockPos blockPos) {
			return this.Place(new ItemPlacementContext(player, stack, blockPos));
		}

		public sealed override IActionResult OnPrimaryUseOnBlock(BlockInteractionResult result) {
			return IActionResult.FAIL;
		}

		public virtual IActionResult Place(ItemPlacementContext context) {
			if (context.player == null) return IActionResult.PASS;

			if (context.player.IsInBlockReach(context.blockPos.GetCenter())) {
				BlockState? placementState = this.GetPlacementState(context);

				if (placementState == null) return IActionResult.FAIL;
				if (!this.CanPlace(context, placementState)) return IActionResult.FAIL;

				context.level.SetBlockState(context.blockPos, placementState);
				ItemStack stack = context.stack;
				stack.Decrement();
				return new IActionResult.Success(new IActionResult.ItemContext(stack));
			} else {
				return IActionResult.PASS;
			}
		}

		protected virtual bool CanPlace(ItemPlacementContext context, BlockState blockState) {
			return true;
		}

		protected virtual BlockState? GetPlacementState(ItemPlacementContext context) {
			return this.block.DefaultState;
		}
	}
}
