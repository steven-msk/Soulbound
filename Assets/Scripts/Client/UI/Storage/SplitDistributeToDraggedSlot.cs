using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;
using System.Collections.Generic;
using System.Linq;

namespace SoulboundBackend.Client.UI {
	public sealed class SplitDistributeToDraggedSlot : SingleDraggedSlotOperation {
		private readonly IItemContainer container;
		private readonly int slotIndex;

		public SplitDistributeToDraggedSlot(int slotIndex, IItemContainer container, SlotDragContext dragCtx)
			: base(container, slotIndex, dragCtx) {
			this.container = container;
			this.slotIndex = slotIndex;
		}

		bool IsStackValid() {
			IItemSlot dragOrigin = container.GetSlot(dragCtx.origin);

			if (slot.HasStack()) {
				return slot.GetStack()!.item == dragOrigin.GetStack()!.item
					&& !slot.GetStack().IsFull();
			}
			return true;
		}

		public override bool CanExecute() {
			return !dragCtx.draggedSlots.Contains(slotIndex) && IsStackValid();
		}

		public override bool Execute() {
			if (!CanExecute()) return false;

			// Clone to preview distribution
			HashSet<int> preview = Enumerable.ToHashSet(new List<int>(dragCtx.draggedSlots) { slotIndex });

			int toSplit = dragCtx.originStack;
			int splitAmount = toSplit / preview.Count;
			if (splitAmount <= 0) return false;

			// Commit the slot to dragged list
			dragCtx.draggedSlots.Add(slotIndex);
			int remainder = toSplit % dragCtx.draggedSlots.Count();

			HashSet<int>.Enumerator enumerator = dragCtx.draggedSlots.GetEnumerator();
			int i = 0;
			while (enumerator.MoveNext()) {
				IItemSlot draggedSlot = container.GetSlot(enumerator.Current);
				int amount = splitAmount + (i < remainder ? 1 : 0);

				if (!draggedSlot.HasStack()) {
					draggedSlot.SetStack(new ItemStack(dragCtx.item, amount));
				}

				bool hasSnapshot = dragCtx.quantitySnapshots.TryGetValue(enumerator.Current, out var snapshot);
				if (hasSnapshot && enumerator.Current != dragCtx.origin) {
					draggedSlot.GetStack()!.SetQuantity(snapshot + amount);
				} else {
					draggedSlot.GetStack()!.SetQuantity(amount);
				}
				i++;
			}
			return true;
		}
	}
}
