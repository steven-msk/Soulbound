using SoulboundBackend.Client.UI.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SoulboundBackend.Client.UI {
	public sealed class PlayerInventoryHandle : MonoBehaviour, IItemContainerHandle {
		private IItemContainerDomain container;

		public void Init(IItemContainerDomain container) => this.container = container;

		public void SetVisible(bool visible) {
			throw new NotImplementedException();
		}

		void IItemSlotEventCallbacks.OnPointerDown(int slotIndex, PointerEventData eventData) {
		}

		void IItemSlotEventCallbacks.OnPointerEnter(int slotIndex, PointerEventData eventData) {
		}

		void IItemSlotEventCallbacks.OnPointerExit(int slotIndex, PointerEventData eventData) {
		}

		void IItemSlotEventCallbacks.OnPointerUp(int slotIndex, PointerEventData eventData) {
		}
	}
}
