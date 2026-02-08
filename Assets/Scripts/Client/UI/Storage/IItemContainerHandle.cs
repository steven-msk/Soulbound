using SoulboundBackend.Client.UI.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace SoulboundBackend.Client.UI {
	public interface IItemContainerHandle : IUIElementHandle, IItemSlotEventCallbacks {
		abstract void IItemSlotEventCallbacks.OnPointerDown(int slotIndex, PointerEventData eventData);
		abstract void IItemSlotEventCallbacks.OnPointerUp(int slotIndex, PointerEventData eventData);
		abstract void IItemSlotEventCallbacks.OnPointerEnter(int slotIndex, PointerEventData eventData);
		abstract void IItemSlotEventCallbacks.OnPointerExit(int slotIndex, PointerEventData eventData);
	}
}
