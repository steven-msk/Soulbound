using SoulboundBackend.Client.Interaction;
using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.BlockSystem.States;

namespace SoulboundBackend.Client.ItemSystem {
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

			itemStack.Decrement();

			return true;
		}
	}
}
