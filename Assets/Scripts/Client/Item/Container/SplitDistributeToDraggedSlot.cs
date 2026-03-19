using SoulboundBackend.Client.ItemSystem;

using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

namespace SoulboundBackend.Client.ItemSystem.Container {
	public sealed class SplitDistributeToDraggedSlot : ISlotOperation {
		private readonly IItemContainer container;
		private readonly int slotIndex;
		private readonly IItemSlot slot;
		private readonly IItemContainerScope scope;

		public SplitDistributeToDraggedSlot(int slotIndex, IItemContainer container, IItemContainerScope scope) {
			this.container = container;
			this.slotIndex = slotIndex;
			this.slot = container.GetSlot(slotIndex);
			this.scope = scope;
		}

		bool IsStackValid() {
			IItemSlot dragOrigin = scope.GetDragState().origin.GetSlot();

			if (slot.HasStack()) {
				return slot.GetStack()!.item == dragOrigin.GetStack()!.item
					&& !slot.GetStack().IsFull();
			}
			return true;
		}

		public bool CanExecute() {
			SlotRef slotRef = new(container, slotIndex);
			return !scope.GetDragState().IsSlotDragged(slotRef) && IsStackValid();
		}

		bool ISlotOperation.Execute() {
			if (!CanExecute()) return false;

			int previewCount = scope.GetDragState().draggedSlots.Count + 1;
			int toSplit = scope.GetDragState().originStack;
			int splitAmount = toSplit / previewCount;
			if (splitAmount <= 0) return false;

			scope.ExtendDrag(container, slotIndex);
			int remainder = toSplit % scope.GetDragState().draggedSlots.Count();

			SortedSet<SlotRef>.Enumerator enumerator = scope.GetDragState().draggedSlots.GetEnumerator();
			int i = 0;
			while (enumerator.MoveNext()) {
				IItemSlot draggedSlot = enumerator.Current.GetSlot();
				int amount = splitAmount + (i < remainder ? 1 : 0);

				if (!draggedSlot.HasStack()) {
					draggedSlot.SetStack(scope.GetDragState().item.CreateStack(amount));
				}
				bool hasSnapshot = scope.GetDragState().quantitySnapshots.TryGetValue(enumerator.Current, out int quantity);
				if (hasSnapshot && !enumerator.Current.Equals(scope.GetDragState().origin)) {
					draggedSlot.GetStack()!.SetQuantity(quantity + amount);
				} else {
					draggedSlot.GetStack()!.SetQuantity(amount);
				}
				i++;
			}
			return true;
		}
	}
}
