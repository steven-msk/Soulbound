using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Client.ItemSystem.Container;
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.Render.Item {
	public sealed class PlayerInventoryHandle : IDisposable {
		private readonly VisualElement root;
		private readonly List<UIToolkitItemSlotHandle> popupHandles = new();
		private readonly List<HotbarSlotHandle> hotbarHandles = new();
		private readonly Inventory inventory;
		private readonly ItemRenderManager itemRenderManager;

		public PlayerInventoryHandle(VisualElement root, Inventory inventory, ItemRenderManager itemRenderManager) {
			this.root = root;
			this.inventory = inventory;
			this.itemRenderManager = itemRenderManager;

			foreach (var slotIndex in inventory.GetPopupSlots()) {
				IItemSlot slot = inventory.GetSlot(slotIndex);
				VisualElement slotElement = this.GetPopup()[slotIndex - Inventory.HOTBAR_SIZE];

				UIToolkitItemSlotHandle handle = new(slotElement, slot, itemRenderManager);
				this.popupHandles.Add(handle);
				this.AddPointerListeners(slotElement, handle, slot);
			}

			foreach (var slotIndex in inventory.GetHotbarSlots()) {
				IItemSlot slot = inventory.GetSlot(slotIndex);
				VisualElement slotElement = this.GetHotbar()[slotIndex];

				HotbarSlotHandle handle = new(slotElement, slot, itemRenderManager);
				this.hotbarHandles.Add(handle);
				this.AddPointerListeners(slotElement, handle, slot);
			}

		}

		private void AddPointerListeners(VisualElement visualElement, UIToolkitItemSlotHandle handle, IItemSlot slot) {
			handle.onPointerDown += evt => this.OnPointerDown(slot, visualElement, evt);
			handle.onPointerUp += evt => this.OnPointerUp(slot, visualElement, evt);
			handle.onPointerEnter += evt => this.OnPointerEnter(slot, visualElement, evt);
			handle.onPointerLeave += evt => this.OnPointerLeave(slot, visualElement, evt);
		}

		private void OnPointerDown(IItemSlot slot, VisualElement visualElement, PointerDownEvent evt) {
			Logger.LogInfo("pointer down: {}", slot.GetIndex());
		}
		
		private void OnPointerUp(IItemSlot slot, VisualElement visualElement, PointerUpEvent evt) {
			Logger.LogInfo("pointer up: {}", slot.GetIndex());
		}

		private void OnPointerEnter(IItemSlot slot, VisualElement visualElement, PointerEnterEvent evt) {
			Logger.LogInfo("pointer enter: {}", slot.GetIndex());
		}

		private void OnPointerLeave(IItemSlot slot, VisualElement visualElement, PointerLeaveEvent evt) {
			Logger.LogInfo("pointer leave: {}", slot.GetIndex());
		}

		private VisualElement GetPopup() => this.root.Q<VisualElement>("Popup");
		private VisualElement GetHotbar() => this.root.Q<VisualElement>("Hotbar");

		public void Dispose() {
			foreach (var slotHandle in this.popupHandles) {
				slotHandle.Dispose();
			}
			this.popupHandles.Clear();
			foreach (var slotHandle in this.hotbarHandles) {
				slotHandle.Dispose();
			}
			this.hotbarHandles.Clear();
		}
	}
}
