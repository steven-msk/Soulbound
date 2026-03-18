using SoulboundBackend.Client.UI.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace SoulboundBackend.Client.UI {
	public interface IItemContainerHandle : IUIElementHandle, IItemSlotHandleCallbacks {
		abstract void IItemSlotHandleCallbacks.OnPointerDown(int slotIndex, PointerEventData eventData);
		abstract void IItemSlotHandleCallbacks.OnPointerUp(int slotIndex, PointerEventData eventData);
		abstract void IItemSlotHandleCallbacks.OnPointerEnter(int slotIndex, PointerEventData eventData);
		abstract void IItemSlotHandleCallbacks.OnPointerExit(int slotIndex, PointerEventData eventData);
	}
}
