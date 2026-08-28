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
		private int mainSlot = 0;
		private readonly ItemSlot[] slots;
		private readonly PlayerEntity player;
		public event Action<int, int> mainSlotChanged;

		public PlayerInventory(PlayerEntity player) {
			IInventory.CreateSimple(this, ref this.slots);
			this.player = player;
		}

		public IEnumerable<int> GetPopup() {
			List<int> list = new();
			for (int i = 0; i < POPUP_ROWS * POPUP_COLUMNS; i++) {
				list.Add(HOTBAR_SIZE + i);
			}
			return list;
		}

		public IEnumerable<int> GetHotbar() {
			List<int> list = new();
			for (int i = 0; i < HOTBAR_SIZE; i++) {
				list.Add(i);
			}
			return list;
		}

		public void Tick() {
			for (int i = 0; i < this.GetSize(); i++) {
				ItemStack stack = this.GetSlot(i).GetStack();
				if (stack.IsEmpty()) continue;

				stack.InventoryTick(this.player.GetLevel(), this.player, i == this.mainSlot ? EquipmentSlot.MAIN_HAND : null);
			}
		}

		public int GetMainSlot() => this.mainSlot;

		public void SetMainSlot(int slot) {
			int previous = this.mainSlot;
			this.mainSlot = slot;
			mainSlotChanged?.Invoke(previous, slot);
		}

		public ItemStack GetMainStack() {
			return this.slots[this.mainSlot].GetStack();
		}

		public ItemStack SetMainStack(ItemStack stack) {
			ItemStack old = this.slots[this.mainSlot].GetStack();
			this.slots[this.mainSlot].SetStack(stack);
			return old;
		}

		public IItemSlot GetSlot(int index) => this.slots[index];

		public IEnumerable<int> GetSlots() => this.slots.Select(s => s.GetIndex());

		public IEnumerable<IItemSlot> GetAllSlots() {
			return this.GetSlots().Select(i => this.GetSlot(i));
		}

		public int GetSize() => HOTBAR_SIZE + POPUP_COLUMNS * POPUP_ROWS;

		public bool CanPlayerUse(PlayerEntity player) => true;
	}
}
