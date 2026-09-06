namespace SoulboundEngine.World.Player {
	using SoulboundEngine.Item;
	using SoulboundEngine.Item.Container;
	using SoulboundEngine.World.Entity;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public sealed class PlayerInventory : IInventory {
		public const int HOTBAR_SIZE = 9;
		public const int POPUP_COLUMNS = 9;
		public const int POPUP_ROWS = 3;
		public const int HELMET_SLOT = 36;
		public const int CHESTPLATE_SLOT = 37;
		public const int LEGGINGS_SLOT = 38;
		public const int BOOTS_SLOT = 39;
		public static readonly Dictionary<int, EquipmentSlot> EQUIPMENT_SLOT_MAPPING = new() {
			[HELMET_SLOT] = EquipmentSlot.HEAD,
			[CHESTPLATE_SLOT] = EquipmentSlot.CHEST,
			[LEGGINGS_SLOT] = EquipmentSlot.LEGS,
			[BOOTS_SLOT] = EquipmentSlot.FEET
		};
		private int mainSlot = 0;
		private readonly ItemSlot[] slots;
		private readonly PlayerEntity player;
		public event Action<int, int> mainSlotChanged;

		public PlayerInventory(PlayerEntity player) {
			this.player = player;
			this.slots = new ItemSlot[this.GetSize()];
			for (int i = 0; i < GetMainAreaSize(); i++) {
				ItemSlot slot = new(this, i);
				this.slots[i] = slot;
			}
			this.slots[HELMET_SLOT] = new ArmorSlot(this, HELMET_SLOT, ArmorType.HELMET, player);
			this.slots[CHESTPLATE_SLOT] = new ArmorSlot(this, CHESTPLATE_SLOT, ArmorType.CHESTPLATE, player);
			this.slots[LEGGINGS_SLOT] = new ArmorSlot(this, LEGGINGS_SLOT, ArmorType.LEGGINGS, player);
			this.slots[BOOTS_SLOT] = new ArmorSlot(this, BOOTS_SLOT, ArmorType.BOOTS, player);
		}

		public IEnumerable<int> GetPopup() {
			List<int> list = new();
			for (int i = 0; i < GetPopupSize(); i++) {
				list.Add(HOTBAR_SIZE + i);
			}
			return list;
		}

		public static int GetPopupSize() => POPUP_COLUMNS * POPUP_ROWS;

		public IEnumerable<int> GetHotbar() {
			List<int> list = new();
			for (int i = 0; i < GetHotbarSize(); i++) {
				list.Add(i);
			}
			return list;
		}

		public static int GetHotbarSize() => HOTBAR_SIZE;

		public void Tick() {
			for (int i = 0; i < GetMainAreaSize(); i++) {
				ItemStack stack = this[i];
				if (stack.IsEmpty()) continue;

				stack.InventoryTick(this.player.GetLevel(), this.player, i == this.mainSlot ? EquipmentSlot.MAIN_HAND : null);
			}
			this.TickArmorSlot(HELMET_SLOT);
			this.TickArmorSlot(CHESTPLATE_SLOT);
			this.TickArmorSlot(LEGGINGS_SLOT);
			this.TickArmorSlot(BOOTS_SLOT);
		}

		private void TickArmorSlot(int slotIndex) {
			ItemSlot slot = this.slots[slotIndex];
			ItemStack stack = slot.GetStack();
			if (stack.IsEmpty()) return;

			if (this.GetEquipmentSlot(slotIndex) is { } equipmentSlot) {
				stack.InventoryTick(this.player.GetLevel(), this.player, equipmentSlot);
			}
		}

		public static int GetMainAreaSize() => GetPopupSize() + GetHotbarSize();

		public int GetMainSlot() => this.mainSlot;

		public void SetMainSlot(int slot) {
			if (slot < 0 || slot >= HOTBAR_SIZE) {
				throw new ArgumentException("Invalid main slot: " + slot);
			}
			int previous = this.mainSlot;
			this.mainSlot = slot;
			mainSlotChanged?.Invoke(previous, slot);
		}

		public ItemStack GetMainStack() => this[this.mainSlot];

		public ItemStack SetMainStack(ItemStack stack) {
			ItemStack old = this[this.mainSlot];
			this.slots[this.mainSlot].SetStack(stack);
			return old;
		}

		public EquipmentSlot? GetEquipmentSlot(int slot) {
			return EQUIPMENT_SLOT_MAPPING.TryGetValue(slot, out EquipmentSlot equipmentSlot) ? equipmentSlot : null;
		}

		public bool IsMainArea(int slot) {
			return slot < GetMainAreaSize();
		}

		public bool IsHotbar(int slot) {
			return slot < GetHotbarSize();
		}

		public bool IsPopup(int slot) {
			return slot >= GetHotbarSize() && slot < GetPopupSize();
		}

		public IItemSlot GetSlot(int index) => this.slots[index];

		public ItemStack this[int index] => this.GetSlot(index).GetStack();

		public IEnumerable<int> GetSlots() => Enumerable.Range(0, this.GetSize());

		public int GetSize() => HOTBAR_SIZE + POPUP_COLUMNS * POPUP_ROWS + 4;

		public bool CanPlayerUse(PlayerEntity player) => true;
	}
}
