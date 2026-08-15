using SoulboundEngine.World.Player;
using System.Collections.Generic;

namespace SoulboundEngine.Item.Container {
	public sealed class SimpleInventory : IInventory {
		private readonly ItemSlot[] slots;
		private readonly int size;

		public SimpleInventory(int size) {
			this.size = size;
			IInventory.CreateSimple(this, ref this.slots);
		}
		private ItemSlot CreateSlot(int index) => new(this, index);

		public IEnumerable<int> GetSlots() {
			List<int> list = new();
			for (int i = 0; i < this.slots.Length; i++) list.Add(i);
			return list;
		}

		public IItemSlot GetSlot(int index) => this.slots[index];

		public int GetSize() => this.size;

		public bool CanPlayerUse(PlayerEntity player) => true;
	}
}
