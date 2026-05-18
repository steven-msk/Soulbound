using System.Collections.Generic;

#nullable enable

namespace SoulboundEngine.Client.ItemSystem.Container {
	public interface IInventoryScope : ITransitStackSource {
		IEnumerable<IItemContainer> GetOpenContainers();
		void AddInventory(Inventory inventory);
		void RemoveInventory(Inventory inventory);
		bool IsOpened(Inventory inventory);

		bool InDragState();
		SlotDragState? GetDragState();

		bool TryBeginDrag(ItemStack stack, SlotRef slotRef, int button);
		void ExtendDrag(SlotRef slotRef);
		void EndDrag();
	}
}
