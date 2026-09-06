namespace SoulboundEngine.Inventory {
	using SoulboundEngine.Item;
	using SoulboundEngine.Item.Container;
	using SoulboundEngine.Recipe;
	using SoulboundEngine.World.Entity;
	using SoulboundEngine.World.Player;
	using System;
	using System.Collections.Generic;
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

		protected override void QuickMove(PlayerEntity player, IItemSlot slot) {
			QuickMove(this, this.playerInventory, slot);
		}

		public static void QuickMove(InventoryScreenHandler handler, PlayerInventory playerInventory, IItemSlot slot) {
			List<int> targetSlots = new();

			ItemStack slotStack = slot.GetStack();
			Equippable? equippable = slotStack.GetComponents().GetOrDefault(ItemComponents.EQUIPPABLE, null!);
			if (equippable != null) {
				foreach ((int slotIndex, EquipmentSlot equipmentSlot) in PlayerInventory.EQUIPMENT_SLOT_MAPPING) {
					if (equipmentSlot.Equals(equippable.slot) && slot.GetIndex() != slotIndex) {
						targetSlots.Add(slotIndex);
					}
				}
			}
			if (playerInventory.IsHotbar(slot.GetIndex())) {
				targetSlots.AddRange(playerInventory.GetPopup());
			} else {
				if (!playerInventory.IsMainArea(slot.GetIndex())) {
					targetSlots.AddRange(playerInventory.GetPopup());
				}
				targetSlots.AddRange(playerInventory.GetHotbar());
			}

			handler.InsertItem(ref slotStack, playerInventory.MapToInstances(targetSlots), false);
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
