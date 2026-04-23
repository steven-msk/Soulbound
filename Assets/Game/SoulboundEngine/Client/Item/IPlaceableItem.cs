using SoulboundEngine.Client.Interaction;
using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Core.Event;

namespace SoulboundEngine.Client.ItemSystem {
	public interface IPlaceableItem : IItemInteractionListener {
		BlockState GetBlockState(ItemStack itemStack);

		bool IItemInteractionListener.ValidateTrigger(InteractionTrigger trigger) {
			return trigger == InteractionTrigger.LeftHold || trigger == InteractionTrigger.LeftClick;
		}

		bool IItemInteractionListener.CanExecute(ItemStack itemStack, in ItemInteraction ctx) {
			BlockPos blockPos = (BlockPos)ctx.player.GetWorldPointerPos();
			return ctx.player.CanPlaceBlockAt(blockPos);
		}

		bool IItemInteractionListener.TryExecute(ItemStack itemStack, in ItemInteraction ctx) {
			BlockState blockState = GetBlockState(itemStack);
			BlockPos blockPos = (BlockPos)ctx.player.GetWorldPointerPos();

			ctx.level.SetBlockState(blockPos, blockState);

			// PROTOTYPICAL
			EventBus.Publish(new BlockPlacedEvent(blockState, blockPos, ctx.level));

			itemStack.Decrement();

			return true;
		}
	}
}
