using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Client.Item.Container;
using SoulboundEngine.Client.Player;
using SoulboundEngine.Client.UI.Screen.Slot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoulboundEngine.Client.UI.Screen {
	public abstract class InventoryScreenHandler {
		private readonly List<SlotRef> slots = new();
		private readonly InventoryScreenHandlerType type;

		protected InventoryScreenHandler(InventoryScreenHandlerType type) {
			this.type = type;
		}

		protected void AddPlayerInventorySlots(PlayerInventory playerInventory) {
			this.slots.AddRange(GetRefs(playerInventory.GetPopup(), playerInventory));
		}

		protected void AddPlayerHotbarSlots(PlayerInventory playerInventory) {
			this.slots.AddRange(GetRefs(playerInventory.GetHotbar(), playerInventory));
		}

		protected void AddPlayerSlots(PlayerInventory playerInventory) {
			this.AddPlayerInventorySlots(playerInventory);
			this.AddPlayerHotbarSlots(playerInventory);
		}

		protected SlotRef AddSlot(IItemSlot slot) {
			SlotRef slotRef = slot.GetRef();
			this.slots.Add(slotRef);
			return slotRef;
		}

		static List<SlotRef> GetRefs(IEnumerable<int> slots, IItemContainer container) {
			return slots.Select(s => new SlotRef(container, s)).ToList();
		}

		/// <summary>
		/// Returns whether the inventory screen handler can be used. <br/>
		/// Subclasses should call this or implement the check itself. <br/>
		/// The implementation should check that the player is near the source position
		/// (like block pos), and that the source (e.g. block) is not destroyed.
		/// </summary>
		public abstract bool CanUse(PlayerEntity player);

		public void OnSlotAction(int slotIndex, int button, PlayerEntity player, SlotActionType actionType) {
			try {
				this.InternalSlotAction(slotIndex, button, player, actionType);
			} catch (Exception e) {
				Logger.LogFatal(e);
			}
		}

		private void InternalSlotAction(int slotIndex, int button, PlayerEntity player, SlotActionType actionType) {
			Logger.LogInfo("slot clicked: {}, {}", slotIndex, actionType);
		}

		public InventoryScreenHandlerType GetHandlerType() => this.type;
	}
}
