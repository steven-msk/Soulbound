using System.Collections.Generic;
using System.Linq;

namespace SoulboundEngine.Client.Item.Container {
	public sealed class SlotDragState {
		public ItemStack stack { get; set; }
		public SlotRef origin { get; init; }
		public HashSet<SlotRef> draggedSlots { get; init; }
		public int button { get; init; }
		public Dictionary<SlotRef, int> countSnapshot { get; init; }
		public bool stackFromOriginSlot { get; init; }
		public HashSet<IInventory> inventories { get; private set; } = new();

		public void ExtendDrag(SlotRef slotRef) {
			this.draggedSlots.Add(slotRef);
			this.inventories.Add(slotRef.inventory);
		}

		public bool IsSlotDragged(SlotRef slotRef) => this.draggedSlots.Contains(slotRef);

		public int CountEligibleDraggedSlots() {
			return this.draggedSlots.Count(this.IsEligible);
		}

		public bool IsEligible(SlotRef slotRef) {
			return this.GetBaseCount(slotRef) < this.stack.item.GetMaxCount();
		}

		public int GetBaseCount(SlotRef slotRef) {
			if (this.stackFromOriginSlot && slotRef.Equals(this.origin)) {
				return 0;
			}

			if (this.countSnapshot.TryGetValue(slotRef, out int count)) {
				return count;
			}
			return 0;
		}

	}
}
