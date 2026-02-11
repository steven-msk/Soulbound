using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;
using System.Collections.Generic;
using System.Linq;

namespace SoulboundBackend.Client.UI {
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
			return !scope.GetDragState().draggedSlots.Contains(slotRef) && IsStackValid();
		}

		private HashSet<int> ResolveDraggedSlots() {
			return scope.GetDragState().draggedSlots
				.Where(r => r.container == this.container)
				.Select(r => r.index)
				.ToHashSet();
		}

		bool ISlotOperation.Execute() {
			if (!CanExecute()) return false;

			// Clone to preview distribution
			HashSet<int> preview = ResolveDraggedSlots();
			preview.Add(slotIndex);

			int toSplit = scope.GetDragState().originStack;
			int splitAmount = toSplit / preview.Count;
			if (splitAmount <= 0) return false;

			// Commit the slot to dragged list
			scope.GetDragState().draggedSlots.Add(new SlotRef(container, slotIndex));
			int remainder = toSplit % scope.GetDragState().draggedSlots.Count();

			HashSet<int>.Enumerator enumerator = ResolveDraggedSlots().GetEnumerator();
			int i = 0;
			while (enumerator.MoveNext()) {
				IItemSlot draggedSlot = container.GetSlot(enumerator.Current);
				int amount = splitAmount + (i < remainder ? 1 : 0);

				if (!draggedSlot.HasStack()) {
					draggedSlot.SetStack(new ItemStack(scope.GetDragState().item, amount));
				}

				bool hasSnapshot = scope.GetDragState().TryGetQuantity(container, slotIndex, out int quantity);
				if (hasSnapshot && enumerator.Current != scope.GetDragState().origin.index) {
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
