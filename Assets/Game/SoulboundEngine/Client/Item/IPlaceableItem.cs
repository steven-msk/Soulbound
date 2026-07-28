using SoulboundEngine.Client.Interaction;
using SoulboundEngine.Client.World.Block;
using SoulboundEngine.Client.World.Block.State;
using SoulboundEngine.Core.Event;

namespace SoulboundEngine.Client.Item {
	public interface IPlaceableItem : IInteractableItem {
		BlockState GetBlockState(ItemStack itemStack);

		bool IInteractableItem.ValidateTrigger(InteractionTrigger trigger) {
			return trigger == InteractionTrigger.LeftHold || trigger == InteractionTrigger.LeftClick;
		}

		bool IInteractableItem.CanExecute(in ItemStack itemStack, in ItemInteraction ctx) {
			BlockPos blockPos = (BlockPos)ctx.player.GetWorldPointerPos();
			return ctx.player.CanPlaceBlockAt(blockPos);
		}

		bool IInteractableItem.TryExecute(ref ItemStack itemStack, in ItemInteraction ctx) {
			BlockState blockState = this.GetBlockState(itemStack);
			BlockPos blockPos = (BlockPos)ctx.player.GetWorldPointerPos();

			ctx.level.SetBlockState(blockPos, blockState);

			// PROTOTYPICAL
			EventBus.Publish(new BlockPlacedEvent(blockState, blockPos, ctx.level));

			itemStack.Decrement();

			return true;
		}
	}
}
