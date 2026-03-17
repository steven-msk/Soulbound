using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.ItemSystem {
	public interface IPlaceableItem : IItemInteractionListener {
		BlockState GetBlockState(ItemStack itemStack);

		bool IItemInteractionListener.ValidateTrigger(ItemInteractionTrigger trigger) {
			return trigger == ItemInteractionTrigger.LeftHold || trigger == ItemInteractionTrigger.LeftClick;
		}

		bool IItemInteractionListener.CanExecute(ItemStack itemStack, ItemInteraction ctx) {
			BlockPos blockPos = (BlockPos)ctx.player.GetWorldPointerPos();
			return ctx.player.CanPlaceBlockAt(blockPos);
		}

		bool IItemInteractionListener.TryExecute(ItemStack itemStack, ItemInteraction ctx) {
			if (!CanExecute(itemStack, ctx)) return false;

			BlockState blockState = GetBlockState(itemStack);
			BlockPos blockPos = (BlockPos)ctx.player.GetWorldPointerPos();
			ctx.level.SetBlockState(blockPos, blockState);

			itemStack.Decrement();

			return true;
		}
	}
}
