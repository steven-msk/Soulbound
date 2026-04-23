
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;

namespace SoulboundEngine.Client.ItemSystem.Container.View {
	public interface IItemSlotEventListener {
		void OnPointerDown(int slotIndex, PointerEventData eventData);
		void OnPointerUp(int slotIndex, PointerEventData eventData);
		void OnPointerEnter(int slotIndex, PointerEventData eventData);
		void OnPointerExit(int slotIndex, PointerEventData eventData);
	}
}
