using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.ItemSystem {
	public interface IPlaceableItem : IItemActionHandler {
		BlockState GetBlockState(ItemStack itemStack);

		bool IItemActionHandler.ValidateTrigger(ItemActionTrigger trigger) {
			return trigger == ItemActionTrigger.LeftHold || trigger == ItemActionTrigger.LeftClick;
		}

		bool IItemActionHandler.CanExecute(ItemStack itemStack, ItemActionContext ctx) {
			BlockPos blockPos = (BlockPos)ctx.player.GetWorldPointerPos();
			return ctx.player.CanPlaceBlockAt(blockPos);
		}

		bool IItemActionHandler.TryExecute(ItemStack itemStack, ItemActionContext ctx) {
			if (!CanExecute(itemStack, ctx)) return false;

			BlockState blockState = GetBlockState(itemStack);
			BlockPos blockPos = (BlockPos)ctx.player.GetWorldPointerPos();
			ctx.level.SetBlockState(blockPos, blockState);

			itemStack.Decrement();

			return true;
		}
	}
}
