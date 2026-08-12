using SoulboundEngine.Client.Interaction;
using SoulboundEngine.Client.Player;
using SoulboundEngine.Client.World;
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
			if (!context.player.IsInBlockReach(context.blockPos.GetCenter())) return IActionResult.PASS;

			BlockState? placementState = this.GetPlacementState(context);

			if (placementState == null || !this.CanPlace(context, placementState)) {
				return IActionResult.FAIL;
			}

			context.level.SetBlockState(context.blockPos, placementState);
			return new IActionResult.Success(new IActionResult.ItemContext(context.stack.DecrementBy(1), false));
		}

		protected virtual bool CanPlace(ItemPlacementContext context, BlockState blockState) {
			return blockState.CanPlaceAt(context.level, context.blockPos);
		}

		protected virtual BlockState? GetPlacementState(ItemPlacementContext context) {
			return this.block.DefaultState;
		}

		public override bool ShouldContinueUse(ItemStack stack, InteractionType type, Level level, PlayerEntity player, BlockPos blockPos) {
			ItemPlacementContext context = new(player, stack, blockPos);
			BlockState? placementState = this.GetPlacementState(context);
			return placementState != null && this.CanPlace(context, placementState);
		}
	}
}
