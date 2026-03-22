using SoulboundBackend.Client.ItemSystem;

using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

namespace SoulboundBackend.Client.ItemSystem.Container {
	public sealed class SplitDistributeToDraggedSlot : ISlotOperation {
		private readonly SlotRef slotRef;
		private readonly IItemSlot slot;
		private readonly IItemContainerScope scope;

		public SplitDistributeToDraggedSlot(SlotRef slotRef, IItemContainerScope scope) {
			this.slotRef = slotRef;
			this.slot = slotRef.GetSlot();
			this.scope = scope;
		}

		bool IsStackValid() {
			return !slot.HasStack() || (slot.GetStack().IsStackableWith(scope.GetDragState().stack)
					&& !slot.GetStack().IsFull());
		}

		public bool CanExecute() {
			if (!scope.InDragState()) return false;

			return !scope.GetDragState().IsSlotDragged(slotRef) && IsStackValid();
		}

		bool ISlotOperation.Execute() {
			if (!CanExecute()) return false;

			int previewCount = scope.GetDragState().draggedSlots.Count + 1;
			int toSplit = scope.GetDragState().stack.quantity;
			int splitAmount = toSplit / previewCount;
			if (splitAmount <= 0) return false;

			scope.ExtendDrag(slotRef);
			int remainder = toSplit % scope.GetDragState().draggedSlots.Count();

			HashSet<SlotRef>.Enumerator enumerator = scope.GetDragState().draggedSlots.GetEnumerator();
			int i = 0;
			while (enumerator.MoveNext()) {
				IItemSlot draggedSlot = enumerator.Current.GetSlot();
				int amount = splitAmount + (i < remainder ? 1 : 0);

				if (!draggedSlot.HasStack()) {
					draggedSlot.SetStack(scope.GetDragState().stack.item.CreateStack(amount));
				} else {
					bool hasSnapshot = scope.GetDragState().quantitySnapshots.TryGetValue(enumerator.Current, out int quantity);
					draggedSlot.GetStack().SetQuantity(hasSnapshot
						? quantity + amount
						: amount
					);
				}
				i++;
			}
			return true;
		}
	}
}
