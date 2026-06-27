using SoulboundEngine.Client.Player;
using System.Collections.Generic;

namespace SoulboundEngine.Client.Item.Container {
	public sealed class SimpleInventory : IInventory {
		private readonly ItemSlot[] slots;

		public SimpleInventory(int size) {
			this.slots = new ItemSlot[size];

			for (int i = 0; i < size; i++) {
				ItemSlot slot = this.CreateSlot(i);
				this.slots[i] = slot;
			}
		}
		private ItemSlot CreateSlot(int index) => new(this, index);

		public IEnumerable<int> GetSlots() {
			List<int> list = new();
			for (int i = 0; i < this.slots.Length; i++) list.Add(i);
			return list;
		}

		public IItemSlot GetSlot(int index) => this.slots[index];

		public int GetSize() => this.slots.Length;

		public bool CanPlayerUse(PlayerEntity player) => true;
	}
}
