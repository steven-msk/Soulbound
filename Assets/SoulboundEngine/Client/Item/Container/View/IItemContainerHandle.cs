using SoulboundEngine.Client.UI;
using UnityEngine.EventSystems;

namespace SoulboundEngine.Client.ItemSystem.Container.View {
	public interface IItemContainerHandle : IUIElementHandle, IItemSlotHandleCallbacks {
		abstract void IItemSlotHandleCallbacks.OnPointerDown(int slotIndex, PointerEventData eventData);
		abstract void IItemSlotHandleCallbacks.OnPointerUp(int slotIndex, PointerEventData eventData);
		abstract void IItemSlotHandleCallbacks.OnPointerEnter(int slotIndex, PointerEventData eventData);
		abstract void IItemSlotHandleCallbacks.OnPointerExit(int slotIndex, PointerEventData eventData);
	}
}
