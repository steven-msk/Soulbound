using System.Collections.Generic;

#nullable enable

namespace SoulboundEngine.Client.ItemSystem.Container {
	public interface IItemContainerScope : ITransitStackSource {
		IEnumerable<IItemContainer> GetOpenContainers();

		bool InDragState();
		SlotDragState? GetDragState();

		bool TryBeginDrag(ItemStack stack, SlotRef slotRef, int button);
		void ExtendDrag(SlotRef slotRef);
		void EndDrag();
	}
}
