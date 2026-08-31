namespace SoulboundEngine.World.Player {
	using SoulboundEngine.Inventory;
	using SoulboundEngine.Item;
	using SoulboundEngine.World.Entity;

#nullable enable

	public class PlayerEquipment : EntityEquipment {
		private readonly PlayerEntity player;

		public PlayerEquipment(PlayerEntity player) {
			this.player = player;
		}

		public override ItemStack Get(EquipmentSlot slot) {
			return slot == EquipmentSlot.MAIN_HAND ? this.GetMainStack() : base.Get(slot);
		}

		public override ItemStack Set(EquipmentSlot slot, ItemStack itemStack) {
			return slot == EquipmentSlot.MAIN_HAND ? this.SetMainStack(itemStack) : base.Set(slot, itemStack);
		}

		private ItemStack GetMainStack() {
			ItemStack transitStack = this.player.GetTransitStack() ?? ItemStack.EMPTY;
			return transitStack.IsEmpty() ? this.player.GetInventory().GetMainStack() : transitStack;
		}

		private ItemStack SetMainStack(ItemStack stack) {
			InventoryScreenHandler? inventoryScreenHandler = this.player.GetInventoryScreenHandler();
			if (inventoryScreenHandler != null && !inventoryScreenHandler.GetTransitStack().IsEmpty()) {
				ItemStack oldStack = inventoryScreenHandler.GetTransitStack();
				inventoryScreenHandler.SetTransitStack(stack);
				return oldStack;
			} else {
				return this.player.GetInventory().SetMainStack(stack);
			}
		}
	}
}
