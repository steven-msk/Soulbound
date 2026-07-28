using SoulboundEngine.Client.Player;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace SoulboundEngine.Client.Item.Container {
	public interface IInventory : IEnumerable<ItemStack> {
		IItemSlot GetSlot(int index);

		IEnumerable<int> GetSlots();

		int GetSize();

		bool CanPlayerUse(PlayerEntity player);

		public IEnumerable<IItemSlot> GetAllSlots() {
			return this.GetSlots().Select(i => this.GetSlot(i));
		}

		virtual void OnOpened(PlayerEntity player) {
		}

		virtual void OnClosed(PlayerEntity player) {
		}

		IEnumerator<ItemStack> IEnumerable<ItemStack>.GetEnumerator() {
			return this.GetAllSlots().Select(s => s.GetStack()).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		protected static void CreateSimple(IInventory inventory, ref ItemSlot[] slots) {
			slots = new ItemSlot[inventory.GetSize()];

			for (int i = 0; i < inventory.GetSize(); i++) {
				ItemSlot slot = new(inventory, i);
				slots[i] = slot;
			}
		}
	}
}
