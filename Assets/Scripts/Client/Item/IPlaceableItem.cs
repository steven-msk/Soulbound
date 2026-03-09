using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.ItemSystem {
	public interface IPlaceableItem : IItemAction {
		BlockState GetBlockState(ItemStack itemStack);

		bool IItemAction.ValidateTrigger(ItemActionTrigger trigger) {
			return trigger == ItemActionTrigger.LeftHold || trigger == ItemActionTrigger.LeftClick;
		}

		bool IItemAction.CanExecute(ItemStack itemStack, ItemActionContext ctx) {
			BlockPos blockPos = (BlockPos)ctx.player.GetWorldPointerPos();
			return ctx.player.CanPlaceBlockAt(blockPos);
		}

		bool IItemAction.TryExecute(ItemStack itemStack, ItemActionContext ctx) {
			if (!CanExecute(itemStack, ctx)) return false;

			BlockState blockState = GetBlockState(itemStack);
			BlockPos blockPos = (BlockPos)ctx.player.GetWorldPointerPos();
			ctx.level.SetBlockState(blockPos, blockState);

			itemStack.Decrement();

			return true;
		}
	}
}
