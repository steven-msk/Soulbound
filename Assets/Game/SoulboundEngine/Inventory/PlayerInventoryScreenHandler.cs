namespace SoulboundEngine.Inventory {
	using SoulboundEngine.Item;
	using SoulboundEngine.Item.Container;
	using SoulboundEngine.Recipe;
	using SoulboundEngine.World.Player;
	using System;
	using System.Linq;

#nullable enable

	public class PlayerInventoryScreenHandler : AbstractRecipeInventoryScreenHandler<StationlessCraftingRecipe, InventoryRecipeInput> {
		public PlayerInventoryScreenHandler(InventoryScreenHandlerType<PlayerInventoryScreenHandler> type, PlayerInventory playerInventory)
			: this(type, playerInventory, InventoryScreenHandlerContext.EMPTY) {
		}

		public PlayerInventoryScreenHandler(InventoryScreenHandlerType<PlayerInventoryScreenHandler> type, PlayerInventory playerInventory, InventoryScreenHandlerContext context) 
			: base(type, playerInventory, context, RecipeType.STATIONLESS) {
			this.AddPlayerSlots(playerInventory);
		}

		public override bool CanUse(PlayerEntity player) => true;

		// TODO: implement quick move for equipment slots

		protected override void QuickMove(PlayerEntity player, IItemSlot slot) {
			IItemSlot[] hotbarSlots = this.playerInventory.GetHotbar().Select(this.playerInventory.GetSlot).ToArray();
			IItemSlot[] popupSlots = this.playerInventory.GetPopup().Select(this.playerInventory.GetSlot).ToArray();

			ItemStack slotStack = slot.GetStack();
			this.InsertItem(ref slotStack, hotbarSlots.Contains(slot) ? popupSlots : hotbarSlots, false);
			slot.SetStack(slotStack);
		}

		public override InventoryRecipeInput GetInput() {
			return new InventoryRecipeInput(this.GetInputSlots());
		}

		public override bool CanInsertIntoSlot(ItemStack itemStack, IItemSlot slot) {
			return CanInsertIntoSlot(base.CanInsertIntoSlot, itemStack, slot);
		}

		public static bool CanInsertIntoSlot(Func<ItemStack, IItemSlot, bool> fallback, ItemStack stack, IItemSlot slot) {
			if (slot is IEquipmentSlot equipmentSlot) {
				Equippable? equippable = stack.GetComponents().GetOrDefault(ItemComponents.EQUIPPABLE, null!);
				return equippable != null && equipmentSlot.GetEquipmentSlot().Equals(equippable.slot);
			}
			return fallback(stack, slot);
		}

		public override IItemSlot[] GetInputSlots() {
			return Enumerable.Range(0, this.playerInventory.GetSize())
				.Select(this.playerInventory.GetSlot)
				.ToArray();
		}
	}
}
