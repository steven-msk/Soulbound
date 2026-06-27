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

		IEnumerator<ItemStack> IEnumerable<ItemStack>.GetEnumerator() {
			return this.GetAllSlots().Select(s => s.GetStack()).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
	}
}
