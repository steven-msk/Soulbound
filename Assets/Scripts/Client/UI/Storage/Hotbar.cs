using SoulboundBackend.Client.ItemSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.UI.Storage {
	public sealed class Hotbar : IItemContainer {
		public const int COLUMNS = 9;
		private readonly InventorySlot[] slots = new InventorySlot[COLUMNS];

		public Hotbar() {
			for (int i = 0; i < COLUMNS; i++) slots[i] = new InventorySlot(this, i);
		}

		public IReadOnlyList<IItemSlot> GetAllSlots() {
			return GetAllSlots_indexed().Select(s => GetSlot(s)).ToList();
		}

		public IReadOnlyList<int> GetAllSlots_indexed() {
			List<int> list = new();
			for (int i = 0; i < COLUMNS; i++) list.Add(i);
			return list;
		}

		public IItemSlot GetSlot(int index) {
			if (index < 9) return slots[index];
			throw new ArgumentException("Hotbar slot index out of range: " + index);
		}

		[Obsolete] IReadOnlyList<IItemSlot> IItemContainer.slots => throw new NotImplementedException();
		[Obsolete] public void OnItemDisplayAdded(ItemDisplay itemDisplay, IItemSlot slot) {
		}
	}
}
