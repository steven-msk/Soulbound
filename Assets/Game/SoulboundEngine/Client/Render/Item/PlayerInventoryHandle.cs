using SoulboundEngine.Client.ItemSystem.Container;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.Render.Item {
	public sealed class PlayerInventoryHandle : IDisposable {
		private readonly List<UIToolkitItemSlotHandle> popupHandles = new();
		private readonly List<HotbarSlotHandle> hotbarHandles = new();
		private readonly Inventory inventory;
		private readonly ItemRenderManager itemRenderManager;
		private readonly IItemContainerScope scope;
		private VisualElement root;
		private int lastClickedSlot;
		private float lastClickTime;
		const float DOUBLE_CLICK_THRESHOLD = 0.15f;
		const int LEFT_BUTTON = 0;
		const int MIDDLE_BUTTON = 2;
		const int RIGHT_BUTTON = 1;

		public PlayerInventoryHandle(Inventory inventory, ItemRenderManager itemRenderManager, IItemContainerScope scope) {
			this.inventory = inventory;
			this.itemRenderManager = itemRenderManager;
			this.scope = scope;
		}

		public void OnBind(VisualElement root) {
			this.root = root;

			foreach (var slotIndex in this.inventory.GetPopupSlots()) {
				IItemSlot slot = this.inventory.GetSlot(slotIndex);
				VisualElement slotElement = this.GetPopup()[slotIndex - Inventory.HOTBAR_SIZE];

				UIToolkitItemSlotHandle handle = new(slotElement, slot, this.itemRenderManager);
				this.popupHandles.Add(handle);
				this.AddPointerListeners(slotElement, handle, slot);
			}

			foreach (var slotIndex in this.inventory.GetHotbarSlots()) {
				IItemSlot slot = this.inventory.GetSlot(slotIndex);
				VisualElement slotElement = this.GetHotbar()[slotIndex];

				HotbarSlotHandle handle = new(slotElement, slot, this.itemRenderManager);
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
			float time = Time.time;
			bool doubleClick = this.lastClickedSlot == slot.GetIndex() && (time - this.lastClickTime) <= DOUBLE_CLICK_THRESHOLD;
			this.lastClickTime = time;
			this.lastClickedSlot = slot.GetIndex();

			int clickButton = evt.button;
			ISlotOperation operation = this.GetClick(slot.GetIndex(), clickButton, doubleClick);

			this.scope.TryBeginDrag(
				this.scope.HasTransitStack()
					? this.scope.GetTransitStack()
					: this.inventory.GetSlot(slot.GetIndex()).GetStack(),
				new SlotRef(this.inventory, slot.GetIndex()),
				clickButton
			);
			operation.Execute();
		}
		
		private void OnPointerUp(IItemSlot slot, VisualElement visualElement, PointerUpEvent evt) {
		}

		private void OnPointerEnter(IItemSlot slot, VisualElement visualElement, PointerEnterEvent evt) {
		}

		private void OnPointerLeave(IItemSlot slot, VisualElement visualElement, PointerLeaveEvent evt) {
		}

		private ISlotOperation GetClick(int slotIndex, int clickButton, bool doubleClick) {
			if (clickButton < 0) return new NoSlotOperation();

			if (clickButton == LEFT_BUTTON) {
				CollectAllItemsToTransit collectToTransit = new(this.scope);

				return doubleClick && collectToTransit.CanExecute()
					? collectToTransit
					: new TransferTransit(this.inventory, slotIndex, this.scope);
			}

			if (clickButton == RIGHT_BUTTON) {
				TransferSingleToSlot transferSingleToSlot = new(this.inventory, slotIndex, this.scope);
				HalveStackFromSlot halveStackFromSlot = new(this.inventory, slotIndex, this.scope);

				if (transferSingleToSlot.CanExecute()) return transferSingleToSlot;
				if (halveStackFromSlot.CanExecute()) return halveStackFromSlot;

				return new NoSlotOperation();
			}
			return new NoSlotOperation();
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
