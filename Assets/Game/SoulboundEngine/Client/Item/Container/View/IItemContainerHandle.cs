using SoulboundEngine.Client.UI;
using UnityEngine.EventSystems;

namespace SoulboundEngine.Client.ItemSystem.Container.View {
	public interface IItemContainerHandle : IUIElementHandle, IItemSlotEventListener {
		abstract void IItemSlotEventListener.OnPointerDown(int slotIndex, PointerEventData eventData);
		abstract void IItemSlotEventListener.OnPointerUp(int slotIndex, PointerEventData eventData);
		abstract void IItemSlotEventListener.OnPointerEnter(int slotIndex, PointerEventData eventData);
		abstract void IItemSlotEventListener.OnPointerExit(int slotIndex, PointerEventData eventData);
	}
}
